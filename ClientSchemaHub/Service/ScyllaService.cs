using Cassandra;
using Cassandra.Data.Linq;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using Dapper;
using System.Data.Common;

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
                    var tableDetails = await GetTableDetailsAsync(connectionDTO, tableName);
                    if (!tableDetailsDictionary.ContainsKey(tableName))
                    {
                        tableDetailsDictionary[tableName] = new List<TableDetailsDTO>();
                    }
                    tableDetailsDictionary[tableName].Add(tableDetails);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching details for table {tableName}: {ex.Message}");
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
                .WithSocketOptions(new SocketOptions().SetConnectTimeoutMillis(30000))
                .WithRetryPolicy(new DefaultRetryPolicy())
                .WithLoadBalancingPolicy(new DCAwareRoundRobinPolicy("datacenter1")); // Replace with your actual datacenter name
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
            return new List<string> { "54.162.28.160" }; // Ensure this IP is correct
        }

        private async Task<List<string>> GetTableNamesAsync(ICluster scyllaDbClient)
        {
            var tableNames = new List<string>();
            Cassandra.ISession session = null;
            try
            {
                session = await scyllaDbClient.ConnectAsync().ConfigureAwait(false);
                var tableNamesQuery = new SimpleStatement($"SELECT table_name FROM system_schema.tables WHERE keyspace_name = '{session.Keyspace}';");
                var tableNamesResult = await session.ExecuteAsync(tableNamesQuery);
                tableNames = tableNamesResult.Select(row => row.GetValue<string>("table_name")).ToList();
            }
            catch (NoHostAvailableException ex)
            {
                Console.WriteLine($"No hosts available: {ex.Message}");
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
            return await GetTableNamesAsync((Cluster)scyllaDbClient);
        }

        //public async Task<TableDetailsDTO> GetTableDetailsAsync(ICluster scyllaDbClient, string tableName)
        //{
        //    var tableDetails = new TableDetailsDTO { TableName = tableName };
        //    try
        //    {
        //        Console.WriteLine(tableName, tableDetails);
        //        using (var session = scyllaDbClient.Connect())
        //        {
        //            var keyspace = session.Keyspace;
        //            if (string.IsNullOrEmpty(keyspace))
        //            {
        //                throw new InvalidOperationException("Keyspace is not set.");
        //            }
        //            if (string.IsNullOrEmpty(tableName))
        //            {
        //                throw new ArgumentException("Table name is null or empty.");
        //            }
        //            Console.WriteLine(session.Cluster.Metadata);
        //            Console.WriteLine(session.Cluster.Metadata);


        //            ICollection<string> tableMetadata = session.Cluster.Metadata.GetTables(keyspace);
        //            if (tableMetadata == null)
        //            {
        //                Console.WriteLine($"Table metadata for {tableName} is null. Check if the table exists in the keyspace {keyspace}.");
        //                return tableDetails;
        //            }
        //            var columnDetailsList = new List<ColumnDetailsDTO>();
        //            foreach (var column in tableMetadata)
        //            {
        //                if (column == null)
        //                {
        //                    Console.WriteLine($"Encountered null column metadata for table {tableName}.");
        //                    continue; // Skip if column is null
        //                }
        //                var columnDetails = new ColumnDetailsDTO
        //                {
        //                    ColumnName = column. ?? "Unknown",
        //                    DataType = column.Type != null ? column.Type.ToString() : "Unknown",
        //                    IsPrimaryKey = false,
        //                    HasForeignKey = false,
        //                    ReferencedTable = null,
        //                    ReferencedColumn = null,
        //                    IsNullable = true
        //                };
        //                columnDetailsList.Add(columnDetails);
        //            }
        //            var additionalColumnDetails = await GetColumnDetailsAsync(session, keyspace, tableName);
        //            // Ensure that the result is not null to avoid potential null reference issues
        //            if (additionalColumnDetails != null)
        //            {
        //                foreach (var detail in additionalColumnDetails)
        //                {
        //                    if (detail == null)
        //                    {
        //                        Console.WriteLine($"Encountered null column detail for table {tableName}.");
        //                        continue; // Skip if detail is null
        //                    }
        //                    // Find the column in the existing list
        //                    var column = columnDetailsList.FirstOrDefault(c => c.ColumnName == detail.ColumnName);
        //                    if (column != null)
        //                    {
        //                        // Update the properties of the column
        //                        column.IsNullable = detail.IsNullable;
        //                        // Update other properties if needed
        //                        // e.g., column.DataType = detail.DataType;
        //                        // Add other property updates as needed
        //                    }
        //                }
        //            }
        //            // Assign the updated list back to tableDetails
        //            tableDetails.Columns = columnDetailsList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error fetching details for table {tableName}: {ex.Message}");
        //        throw;
        //    }
        //    return tableDetails;
        //}




        public async Task<TableDetailsDTO> GetTableDetailsAsync(DBConnectionDTO connectionDTO, string tableName)
        {
            var tableDetails = new TableDetailsDTO { TableName = tableName };

            try
            {
                // Create a new cluster and session for each call
                var cluster = Cluster.Builder()
                                     .AddContactPoint(connectionDTO.IPAddress)
                                     .Build();

                using (var session = cluster.Connect(connectionDTO.Keyspace))
                {
                    // Get metadata from the session's cluster instance
                    var tableMetadata = session.Cluster.Metadata.GetTable(session.Keyspace, tableName);

                    if (tableMetadata != null)
                    {
                        tableDetails.Columns = tableMetadata.TableColumns.Select(column => new ColumnDetailsDTO
                        {
                            ColumnName = column.Name,
                            DataType = column.Type.ToString() // ScyllaDB's type representation
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


        private async Task<List<ColumnDetailsDTO>> GetColumnDetailsAsync(Cassandra.ISession session, string keyspace, string tableName)
        {
            var columnsQuery = $@"
SELECT column_name, type, is_nullable
FROM system_schema.columns
WHERE keyspace_name = '{keyspace}' AND table_name = '{tableName}'";
            // Execute the query to get column details
            var result = await session.ExecuteAsync(new SimpleStatement(columnsQuery));
            var columnDetailsList = result.Select(row => new ColumnDetailsDTO
            {
                ColumnName = row.GetValue<string>("column_name"),
                DataType = row.GetValue<string>("type"),
                IsNullable = row.GetValue<string>("is_nullable") == "YES"
            }).ToList();

            // Handle null values for columns that are expected to be Double
            foreach (var column in columnDetailsList)
            {
                if (column.DataType == "double" && column.IsNullable)
                {
                    // Assuming you want to represent null Double values as 0.0
                    column.DataType = "double?"; // or Nullable<Double>
                }
            }

            return columnDetailsList;
        }

        private async Task<TableDetailsDTO> GetTableDetailsAsync(DbConnection connection, string tableName)
        {
            var tableDetails = new TableDetailsDTO { TableName = tableName };
            string columnsQuery = @"
SELECT
column_name AS ColumnName,
data_type AS DataType,
CASE WHEN IS_NULLABLE = 'NO' THEN 0 ELSE 1 END AS IsNullable,
(SELECT COUNT(1) > 0 FROM information_schema.key_column_usage kcu
WHERE kcu.constraint_name IN (
SELECT tc.constraint_name FROM information_schema.table_constraints tc
WHERE tc.table_name = @TableName AND tc.constraint_type = 'PRIMARY KEY'
) AND kcu.table_name = @TableName AND kcu.column_name = c.column_name) AS IsPrimaryKey,
EXISTS (
SELECT 1 FROM information_schema.key_column_usage kcu
JOIN information_schema.table_constraints tc ON tc.constraint_name = kcu.constraint_name
WHERE tc.table_name = @TableName AND tc.constraint_type = 'FOREIGN KEY'
AND kcu.column_name = c.column_name
) AS HasForeignKey,
(SELECT ccu.table_name FROM information_schema.key_column_usage kcu
JOIN information_schema.constraint_column_usage ccu ON ccu.constraint_name = kcu.constraint_name
JOIN information_schema.table_constraints tc ON tc.constraint_name = kcu.constraint_name
WHERE tc.table_name = @TableName AND tc.constraint_type = 'FOREIGN KEY'
AND kcu.column_name = c.column_name) AS ReferencedTable,
(SELECT ccu.column_name FROM information_schema.key_column_usage kcu
JOIN information_schema.constraint_column_usage ccu ON ccu.constraint_name = kcu.constraint_name
JOIN information_schema.table_constraints tc ON tc.constraint_name = kcu.constraint_name
WHERE tc.table_name = @TableName AND tc.constraint_type = 'FOREIGN KEY'
AND kcu.column_name = c.column_name) AS ReferencedColumn
FROM information_schema.columns c
WHERE table_name = @TableName";
            try
            {
                var columns = await connection.QueryAsync<dynamic>(columnsQuery, new { TableName = tableName });
                // Map results manually to handle null values
                var columnDetails = columns.Select(row => new ColumnDetailsDTO
                {
                    ColumnName = row.ColumnName ?? "Unknown",
                    DataType = row.DataType ?? "Unknown",
                    IsNullable = row.IsNullable != null ? Convert.ToBoolean(row.IsNullable) : false,
                    IsPrimaryKey = row.IsPrimaryKey != null ? Convert.ToBoolean(row.IsPrimaryKey) : false,
                    HasForeignKey = row.HasForeignKey != null ? Convert.ToBoolean(row.HasForeignKey) : false,
                    ReferencedTable = row.ReferencedTable ?? "Unknown",
                    ReferencedColumn = row.ReferencedColumn ?? "Unknown"
                }).ToList();
                tableDetails.Columns = columnDetails;
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