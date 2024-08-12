using Cassandra;
using Cassandra.Mapping;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Cassandra.Data.Linq;
namespace ClientSchemaHub.Service
{
    public class ScyllaService : IScyllaService
    {
        public async Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsForAllTablesAsync(DBConnectionDTO connectionDTO)
        {
            var scyllaDbClient = GetScyllaDbClient(connectionDTO);
            var tableNames = await GetTableNamesAsync(scyllaDbClient);
            var tableDetailsDictionary = new Dictionary<string, List<TableDetailsDTO>>();
            foreach (var tableName in tableNames)
            {
                try
                {
                    var tableDetails = await GetTableDetailsAsync(scyllaDbClient, tableName);
                    if (!tableDetailsDictionary.ContainsKey(tableName))
                    {
                        tableDetailsDictionary[tableName] = new List<TableDetailsDTO>();
                    }
                    tableDetailsDictionary[tableName].Add(tableDetails);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching details for table {tableName}: {ex.Message}");
                    // Optionally log the error or handle it based on requirements
                }
            }
            return tableDetailsDictionary;
        }
        private ICluster GetScyllaDbClient(DBConnectionDTO connectionDTO)
        {
            var ec2Instance = connectionDTO.Ec2Instance;
            var region = connectionDTO.Region;
            var hosts = ResolveHosts(ec2Instance, region);
            var builder = Cluster.Builder()
                .AddContactPoints(hosts)
                .WithCredentials(connectionDTO.AccessKey, connectionDTO.SecretKey)
                .WithDefaultKeyspace(connectionDTO.Keyspace)
                .WithSocketOptions(new SocketOptions().SetConnectTimeoutMillis(30000)) // Increase socket connect timeout
                .WithRetryPolicy(new DefaultRetryPolicy()) // Use default retry policy
                .WithLoadBalancingPolicy(new DCAwareRoundRobinPolicy(region)); // Optional: Use appropriate load balancing policy
            try
            {
                return builder.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error building ScyllaDB client: {ex.Message}");
                throw;
            }
        }
        private IEnumerable<string> ResolveHosts(string ec2Instance, string region)
        {
            // Replace with actual hostname resolution logic
            return new List<string> { "54.242.20.230" }; // Ensure this IP is correct
        }
        private async Task<List<string>> GetTableNamesAsync(ICluster scyllaDbClient)
        {
            var tableNames = new List<string>();
            Cassandra.ISession session = null;
            try
            {
                // Connect to the cluster
                session = await scyllaDbClient.ConnectAsync().ConfigureAwait(false);
                // Get the metadata for the connected cluster
                var metadata = session.Cluster.Metadata;
                // Get the keyspace metadata (assuming a keyspace is already selected)
                var keyspaceMetadata = metadata.GetKeyspace(session.Keyspace);
                // Get the table names from the keyspace metadata
                if (keyspaceMetadata != null)
                {
                    tableNames.AddRange(keyspaceMetadata.GetTablesNames());
                }
                else
                {
                    Console.WriteLine("Keyspace metadata is null.");
                    //}
                }
            }
            catch (NoHostAvailableException ex)
            {
                Console.WriteLine($"No hosts available: {ex.Message}");
                // Handle or log as needed
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting table names: {ex.Message}");
                throw;
            }
            finally
            {
                session?.Dispose();
            }
            return tableNames;
        }
        public async Task<List<string>> GetTableNamesAsync(DBConnectionDTO connectionDTO)
        {
            var scyllaDbClient = GetScyllaDbClient(connectionDTO);
            return await GetTableNamesAsync((Cluster)scyllaDbClient); // Cast to Cluster
        }
        private async Task<TableDetailsDTO> GetTableDetailsAsync(ICluster scyllaDbClient, string tableName)
        {
            var tableDetails = new TableDetailsDTO { TableName = tableName };
            try
            {
                using (var session = scyllaDbClient.Connect())
                {
                    var tableMetadata = session.Cluster.Metadata.GetTable(session.Keyspace, tableName);
                    if (tableMetadata != null)
                    {
                        tableDetails.Columns = tableMetadata.TableColumns.Select(column => new ColumnDetailsDTO
                        {
                            ColumnName = column.Name,
                            DataType = column.Type.ToString()
                        }).ToList();
                    }
                    else
                    {
                        Console.WriteLine($"Table {tableName} does not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching details for table {tableName}: {ex.Message}");
                throw;
            }
            return tableDetails;
        }
        private async Task<TableDetailsDTO> GetTableDetailsAsync(DbConnection connection, string tableName)
        {
            var tableDetails = new TableDetailsDTO { TableName = tableName };
            string columnsQuery = @"
             SELECT
                 column_name AS ColumnName,
                 data_type AS DataType,
                 CASE WHEN IS_NULLABLE = 'NO' THEN 0 ELSE 1 END AS IsNullable,
                 (SELECT COUNT(1) > 0
                  FROM information_schema.key_column_usage kcu
                  WHERE kcu.constraint_name IN (
                      SELECT tc.constraint_name
                      FROM information_schema.table_constraints tc
                      WHERE tc.table_name = @TableName
                      AND tc.constraint_type = 'PRIMARY KEY'
                  )
                  AND kcu.table_name = @TableName
                  AND kcu.column_name = c.column_name
                 ) AS IsPrimaryKey,
                 EXISTS (
                     SELECT 1
                     FROM information_schema.key_column_usage kcu
                     JOIN information_schema.table_constraints tc ON tc.constraint_name = kcu.constraint_name
                     WHERE tc.table_name = @TableName
                     AND tc.constraint_type = 'FOREIGN KEY'
                     AND kcu.column_name = c.column_name
                 ) AS HasForeignKey,
                 (SELECT ccu.table_name
                  FROM information_schema.key_column_usage kcu
                  JOIN information_schema.constraint_column_usage ccu ON ccu.constraint_name = kcu.constraint_name
                  JOIN information_schema.table_constraints tc ON tc.constraint_name = kcu.constraint_name
                  WHERE tc.table_name = @TableName
                  AND tc.constraint_type = 'FOREIGN KEY'
                  AND kcu.column_name = c.column_name
                 ) AS ReferencedTable,
                 (SELECT ccu.column_name
                  FROM information_schema.key_column_usage kcu
                  JOIN information_schema.constraint_column_usage ccu ON ccu.constraint_name = kcu.constraint_name
                  JOIN information_schema.table_constraints tc ON tc.constraint_name = kcu.constraint_name
                  WHERE tc.table_name = @TableName
                  AND tc.constraint_type = 'FOREIGN KEY'
                  AND kcu.column_name = c.column_name
                 ) AS ReferencedColumn
             FROM information_schema.columns c
             WHERE table_name = @TableName";
            try
            {
                var columns = await connection.QueryAsync<ColumnDetailsDTO>(columnsQuery, new { TableName = tableName });
                tableDetails.Columns = columns.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching columns for table {tableName}: {ex.Message}");
                throw;
            }
            return tableDetails;
        }
    }
}