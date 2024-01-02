using System.Data.Common;
using Dapper;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using Npgsql;

namespace ClientSchemaHub.Service
{
    public class PostgreSQLService : IPostgreSQLService
    {
        // Register the Npgsql provider
        public PostgreSQLService()
        {
            DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
        }

        public async Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsForAllTablesAsync(DBConnectionDTO dBConnection)
        {
            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(dBConnection.Provider);

                string connectionString = BuildConnectionString(dBConnection);

                using (DbConnection connection = factory.CreateConnection())
                {
                    if (connection == null)
                    {
                        throw new InvalidOperationException("Provider not supported");
                    }

                    connection.ConnectionString = connectionString;
                    await connection.OpenAsync();

                    List<string> tableNames = await GetTableNamesAsync(connection);
                    Dictionary<string, List<TableDetailsDTO>> tableDetailsDictionary = new Dictionary<string, List<TableDetailsDTO>>();

                    foreach (var tableName in tableNames)
                    {
                        TableDetailsDTO tableDetails = await GetTableDetailsAsync(connection, tableName);

                        if (!tableDetailsDictionary.ContainsKey(tableName))
                        {
                            tableDetailsDictionary[tableName] = new List<TableDetailsDTO>();
                        }

                        tableDetailsDictionary[tableName].Add(tableDetails);
                    }

                    return tableDetailsDictionary;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        //Get all entity names
        public async Task<List<string>> GetTableNamesAsync(DBConnectionDTO dBConnection)
        {
            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(dBConnection.Provider);

                string connectionString = BuildConnectionString(dBConnection);

                using (DbConnection connection = factory.CreateConnection())
                {
                    if (connection == null)
                    {
                        throw new InvalidOperationException("Provider not supported");
                    }

                    connection.ConnectionString = connectionString;
                    await connection.OpenAsync();

                    return await GetTableNamesAsync(connection);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        private async Task<List<string>> GetTableNamesAsync(DbConnection connection)
        {
            const string query = "SELECT table_name FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_schema = 'public'";

            // Use Dapper to execute the query asynchronously and retrieve results dynamically
            return (await connection.QueryAsync<string>(query)).AsList();
        }

        
        //Get Table column properties
        public async Task<TableDetailsDTO> GetTableDetailsAsync(DBConnectionDTO dBConnection, string tableName)
        {
            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(dBConnection.Provider);

                string connectionString = BuildConnectionString(dBConnection);

                using (DbConnection connection = factory.CreateConnection())
                {
                    if (connection == null)
                    {
                        throw new InvalidOperationException("Provider not supported");
                    }

                    connection.ConnectionString = connectionString;
                    await connection.OpenAsync();

                    return await GetTableDetailsAsync(connection, tableName);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        private async Task<TableDetailsDTO> GetTableDetailsAsync(DbConnection connection, string tableName)
        {
            TableDetailsDTO tableDetails = new TableDetailsDTO { TableName = tableName };

            const string columnsQuery = @"
    SELECT 
        column_name AS ColumnName,
        data_type AS DataType,
        (
            SELECT 
                COUNT(1) > 0
            FROM 
                information_schema.key_column_usage kcu
            WHERE 
                kcu.constraint_name IN (
                    SELECT 
                        tc.constraint_name
                    FROM 
                        information_schema.table_constraints tc
                    WHERE 
                        tc.table_name = @TableName
                        AND tc.constraint_type = 'PRIMARY KEY'
                )
                AND kcu.table_name = @TableName
                AND kcu.column_name = c.column_name
        ) AS IsPrimaryKey,
        EXISTS (
            SELECT 1
            FROM 
                information_schema.key_column_usage kcu
                JOIN information_schema.table_constraints tc ON tc.constraint_name = kcu.constraint_name
            WHERE 
                tc.table_name = @TableName
                AND tc.constraint_type = 'FOREIGN KEY'
                AND kcu.column_name = c.column_name
        ) AS HasForeignKey,
        (
            SELECT 
                ccu.table_name
            FROM 
                information_schema.key_column_usage kcu
                JOIN information_schema.constraint_column_usage ccu ON ccu.constraint_name = kcu.constraint_name
                JOIN information_schema.table_constraints tc ON tc.constraint_name = kcu.constraint_name
            WHERE 
                tc.table_name = @TableName
                AND tc.constraint_type = 'FOREIGN KEY'
                AND kcu.column_name = c.column_name
        ) AS ReferencedTable,
        (
            SELECT 
                ccu.column_name
            FROM 
                information_schema.key_column_usage kcu
                JOIN information_schema.constraint_column_usage ccu ON ccu.constraint_name = kcu.constraint_name
                JOIN information_schema.table_constraints tc ON tc.constraint_name = kcu.constraint_name
            WHERE 
                tc.table_name = @TableName
                AND tc.constraint_type = 'FOREIGN KEY'
                AND kcu.column_name = c.column_name
        ) AS ReferencedColumn
    FROM 
        information_schema.columns c
    WHERE 
        table_name = @TableName";

            try
            {
                // Execute the query
                var columns = await connection.QueryAsync<ColumnDetailsDTO>(columnsQuery, new { TableName = tableName });

                // Set Columns property in TableDetailsDTO
                tableDetails.Columns = columns.ToList();

                return tableDetails;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                throw new ArgumentException(ex.Message);
            }
        }


        // Get all rescords from the specific table
        public async Task<List<dynamic>> GetPrimaryColumnDataAsync(DBConnectionDTO dBConnection, string tableName)
        {
            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(dBConnection.Provider);

                string connectionString = BuildConnectionString(dBConnection);

                using (DbConnection connection = factory.CreateConnection())
                {
                    if (connection == null)
                    {
                        throw new InvalidOperationException("Provider not supported");
                    }

                    connection.ConnectionString = connectionString;
                    await connection.OpenAsync();

                    // Query to get the primary key column name
                    string primaryKeyQuery = $@"
                SELECT column_name
                FROM information_schema.table_constraints tc
                JOIN information_schema.key_column_usage kcu
                ON tc.constraint_name = kcu.constraint_name
                WHERE constraint_type = 'PRIMARY KEY'
                AND kcu.table_name = '{tableName}'";

                    string primaryKeyColumnName = await connection.QueryFirstOrDefaultAsync<string>(primaryKeyQuery);

                    // If a primary key column is found, query for its data
                    if (!string.IsNullOrEmpty(primaryKeyColumnName))
                    {
                        string query = $"SELECT {primaryKeyColumnName} FROM {tableName}";
                        return (await connection.QueryAsync(query)).ToList();
                    }
                    else
                    {
                        throw new InvalidOperationException($"Table '{tableName}' does not have a primary key.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        // Build Connection string
        private string BuildConnectionString(DBConnectionDTO connectionDTO)
        {
            // Build and return the connection string based on the DTO properties
            // This is just a simple example; in a real-world scenario, you would want to handle this more securely
            return $"Host={connectionDTO.HostName};Database={connectionDTO.DataBase};Username={connectionDTO.UserName};Password={connectionDTO.Password}";
        }
    }
}
