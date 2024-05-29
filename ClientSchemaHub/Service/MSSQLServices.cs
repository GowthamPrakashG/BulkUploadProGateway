using System.Data;
using System.Data.Common;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using Dapper;
using Microsoft.Data.SqlClient;


namespace ClientSchemaHub.Service
{
    public class MSSQLService : IMSSQLService
    {
        public MSSQLService()
        {
            // Register the MS SQL provider
            DbProviderFactories.RegisterFactory("MS SQL", SqlClientFactory.Instance);
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
            //       const string query = "SHOW TABLES";
            const string query = @"
            SELECT TABLE_NAME 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_TYPE = 'BASE TABLE'";

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

            string columnsQuery = @"
    SELECT 
        column_name AS ColumnName,
        data_type AS DataType,
        CASE WHEN IS_NULLABLE = 'NO' THEN 0 ELSE 1 END AS IsNullable, -- Convert to boolean
        (
            SELECT 
                CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
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
        (
            SELECT 
                CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
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
        public async Task<List<object>> GetPrimaryColumnDataAsync(DBConnectionDTO dBConnection, string tableName)
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

                    // Execute the query to get the primary key column name
                    string primaryKeyColumnName = await connection.QueryFirstOrDefaultAsync<string>(primaryKeyQuery);

                    // If a primary key column is found, query for its data
                    if (!string.IsNullOrEmpty(primaryKeyColumnName))
                    {
                        string query = $"SELECT {primaryKeyColumnName} FROM {tableName}";

                        // Fetch the list of objects
                        var results = await connection.QueryAsync<dynamic>(query);

                        // Extract the values and return as a list of objects
                        var idList = results.Select(result =>
                        {
                            // Use dynamic object to access column values
                            IDictionary<string, object> dynamicResult = result;

                            // Try to retrieve the column value by name
                            object primaryKeyValue = null;
                            if (dynamicResult != null && dynamicResult.ContainsKey(primaryKeyColumnName))
                            {
                                primaryKeyValue = dynamicResult[primaryKeyColumnName];
                            }

                            // Ensure the value is not null before returning
                            return primaryKeyValue;
                        }).ToList();


                        return idList;
                    }
                    else
                    {
                        // If no primary key is found, return an empty list
                        return new List<object>();
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Create a command to execute SQL statements
                    using (SqlCommand command = new SqlCommand())
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

        //Insert data
        public async Task<bool> Insertdata(DBConnectionDTO connectionDTO, List<Dictionary<string, string>>? convertedDataList, List<ColumnMetaDataDTO>? booleancolumns, string tablename)
        {
            string identityColumnName = "StudentId"; // Replace "StudentId" with your actual identity column name

            if (convertedDataList == null || convertedDataList.Count == 0)
            {
                throw new ArgumentException("convertedDataList cannot be null or empty");
            }

            if (string.IsNullOrEmpty(tablename))
            {
                throw new ArgumentException("tablename cannot be null or empty");
            }

            foreach (var data2 in convertedDataList)
            {
                // Process boolean columns if booleancolumns is not null or empty
                if (booleancolumns != null && booleancolumns.Count > 0)
                {
                    foreach (var boolvalue in booleancolumns)
                    {
                        if (data2.ContainsKey(boolvalue.ColumnName))
                        {
                            var value = data2[boolvalue.ColumnName];
                            if (!string.IsNullOrEmpty(value))
                            {
                                if (value != "0" && value != "1")
                                {
                                    data2[boolvalue.ColumnName] = value.ToLower() == boolvalue.True.ToLower() ? "1" : "0";
                                }
                            }
                            else
                            {
                                data2[boolvalue.ColumnName] = "0";
                            }
                        }
                    }
                }

                // Exclude the identity column from the data dictionary
                var dataWithoutIdentity = data2.Where(kvp => !kvp.Key.Equals(identityColumnName, StringComparison.OrdinalIgnoreCase))
                                               .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // Log the data without identity column
                Console.WriteLine("Data without identity column:");
                foreach (var kvp in dataWithoutIdentity)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }

                try
                {
                    string connectionString = BuildConnectionString(connectionDTO);

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = connection;

                            string columns = string.Join(", ", dataWithoutIdentity.Keys.Select(k => $"[{k}]")); // Use square brackets for column names
                            string values = string.Join(", ", dataWithoutIdentity.Values.Select(v => $"'{v}'")); // Wrap values in single quotes for strings

                            string query = $"INSERT INTO [{tablename}] ({columns}) VALUES ({values})"; // Use square brackets for table name

                            // Log the query
                            Console.WriteLine($"Query: {query}");

                            cmd.CommandText = query;
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await connection.CloseAsync();
                    }
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception($"SQL Error: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex)
                {
                    throw new Exception($"General Error: {ex.Message}", ex);
                }
            }
            return true;
        }


        //Check table exists
        public async Task<bool> IsTableExists(DBConnectionDTO dBConnection, string tableName)
        {
            try
            {
                string connectionString = BuildConnectionString(dBConnection);
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string not found in the session.");
                }
                using (var dbConnection = new SqlConnection(connectionString))
                {
                    // Open the connection
                    dbConnection.Open();

                    // Use Dapper to execute a parameterized query to check if the table exists
                    string query = @"
                SELECT CASE 
                    WHEN EXISTS (
                        SELECT 1 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = @TableName
                    ) 
                    THEN 1 
                    ELSE 0 
                END";

                    bool tableExists = dbConnection.QueryFirstOrDefault<bool>(query, new { TableName = tableName });

                    return tableExists;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Get Table data
        public async Task<List<dynamic>> GetTabledata(DBConnectionDTO dBConnection, string tableName)
        {
            string connectionString = BuildConnectionString(dBConnection);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string not found in the session.");
            }

            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();

                // Dynamically query the table based on the provided table name
                string rowDataQuery = $"SELECT * FROM [{tableName}]";  // SQL Server compatible query

                // Use Dapper to execute the query and return the results
                var rows = (await dbConnection.QueryAsync<dynamic>(rowDataQuery)).ToList();

                return rows;
            }
        }


        // Build Connection string
        private string BuildConnectionString(DBConnectionDTO connectionDTO)
        {
            // Build and return the connection string based on the DTO properties
            // This is just a simple example; in a real-world scenario, you would want to handle this more securely
            return $"Server={connectionDTO.HostName};Database={connectionDTO.DataBase};User Id={connectionDTO.UserName};Password={connectionDTO.Password};TrustServerCertificate=true";
        }
    }
}


