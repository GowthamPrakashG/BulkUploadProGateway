using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using MySql.Data.MySqlClient;

namespace ClientSchemaHub.Service
{
    public class MySQLService : IMySQLService
    {
        public MySQLService()
        {
            // Register the MySQL provider
            DbProviderFactories.RegisterFactory("mysql", MySqlClientFactory.Instance);
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
        // Get all entity names
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
            const string query = "SHOW TABLES";

            // Use Dapper to execute the query asynchronously and retrieve results dynamically
            return (await connection.QueryAsync<string>(query)).AsList();
        }

        // Get Table column properties
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

        // Get primary column data from the specific table
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

        //create table
        public async Task<bool> ConvertAndCallCreateTableModel(DBConnectionDTO connectionDTO, string createQuery)
        {
            try
            {
                // Create a MySQL connection string
                string connectionString = $"Server={connectionDTO.HostName};Database={connectionDTO.DataBase};User Id={connectionDTO.UserName};Password={connectionDTO.Password}";

                // Create a new MySQL connection
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Create a command to execute SQL statements
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;

                        // Generate the SQL statement to create the table
                        string createTableSql = createQuery;

                        // Set the SQL statement
                        command.CommandText = createTableSql;

                        // Execute the SQL statement
                        await command.ExecuteNonQueryAsync();
                    }
                    await connection.CloseAsync();
                    return true;
                }

                // Now you can use the mapTable object as needed.
            }
            catch (Exception ex)
            {
                // Handle exceptions
                throw new ArgumentException(ex.Message);
            }
        }

        private  string BuildConnectionString(DBConnectionDTO connectionDTO)
        {
            // Build and return the connection string based on the DTO properties
            // This is just a simple example; in a real-world scenario, you would want to handle this more securely
            return $"Server={connectionDTO.HostName};Database={connectionDTO.DataBase};User ID={connectionDTO.UserName};Password={connectionDTO.Password};";
        }
    }
}
