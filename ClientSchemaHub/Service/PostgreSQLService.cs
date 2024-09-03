using System.Data.Common;
using Dapper;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using Npgsql;
using System.Data;
using System.Globalization;
using Spire.Xls;

namespace ClientSchemaHub.Service
{
    public class PostgreSQLService : IPostgreSQLService
    {
        // Register the Npgsql provider
        public PostgreSQLService()
        {
            DbProviderFactories.RegisterFactory("postgresql", NpgsqlFactory.Instance);
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

            string columnsQuery = @"
    SELECT 
        column_name AS ColumnName,
        data_type AS DataType,
        CASE WHEN IS_NULLABLE = 'NO' THEN 0 ELSE 1 END AS IsNullable, -- Convert to boolean
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


        // Get PrimaryKey records from the specific table
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
                        string query = $"SELECT \"{primaryKeyColumnName}\" FROM \"{tableName}\";";


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



        //create Table
        public async Task<bool> ConvertAndCallCreateTableModel(DBConnectionDTO connectionDTO, string createquery)
        {
            try
            {

                // Create a PostgreSQL connection string
                string connectionString = $"Host={connectionDTO.HostName};Database={connectionDTO.DataBase};Username={connectionDTO.UserName};Password={connectionDTO.Password}";

                // Create a new PostgreSQL connection
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Create a command to execute SQL statements
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.Connection = connection;

                        // Generate the SQL statement to create the table
                        string createTableSql = createquery;

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
            List<Dictionary<string, string>> dataToRemove = new List<Dictionary<string, string>>();

            foreach (var data2 in convertedDataList)
            {
                foreach (var boolvalue in booleancolumns)
                {
                    if (data2.ContainsKey(boolvalue.ColumnName))
                    {
                        // Update the value for the specific key
                        var value = data2[boolvalue.ColumnName];
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (value != "0")
                            {
                                if (value != "1")
                                {
                                    if (value.ToLower() == boolvalue.True.ToLower())
                                    {
                                        data2[boolvalue.ColumnName] = "1";
                                    }
                                    else
                                    {
                                        data2[boolvalue.ColumnName] = "0";
                                    }
                                }
                            }
                        }
                        else
                        {
                            data2[boolvalue.ColumnName] = "0";
                        }
                    }
                }
                try
                {
                    string connectionString = BuildConnectionString(connectionDTO) + ";Include Error Detail=true";

                    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                    {

                        connection.Open();

                        using (NpgsqlCommand cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = connection;
                            // Build the INSERT statement

                            string columns2 = string.Join(", ", data2.Keys.Select(k => $"\"{k}\"")); // Use double quotes for case-sensitive column names

                            string values = string.Join(", ", data2.Values.Select(v => $"'{v}'")); // Wrap values in single quotes for strings

                            string query = $"INSERT INTO \"{tablename}\" ({columns2}) VALUES ({values})"; // Use double quotes for case-sensitive table name

                            cmd.CommandText = query;

                            cmd.ExecuteNonQuery();

                            dataToRemove.Add(data2);

                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
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
                using (var dbConnection = new NpgsqlConnection(connectionString))
                {
                    // Open the connection
                    dbConnection.Open();

                    // Use Dapper to execute a parameterized query to check if the table exists
                    string query = "SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = @TableName)";
                    bool tableExists = dbConnection.QueryFirstOrDefault<bool>(query, new { TableName = tableName });

                    return tableExists;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        //Get Table datas
        public async Task<List<dynamic>> GetTabledata(DBConnectionDTO dBConnection, string tableName)
        {
            string connectionString = BuildConnectionString(dBConnection);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string not found in the session.");
            }

            using (IDbConnection dbConnection = new NpgsqlConnection(connectionString))
            {
                dbConnection.Open();

                // Dynamically query the table based on the provided table name and EntityColumnName
                string rowDataQuery = $"SELECT * FROM public.\"{tableName}\"";

                // Use Dapper to execute the query and return the results
                var rows = dbConnection.Query(rowDataQuery).ToList();

                return rows;
            }
        }

        // Build Connection string
        private string BuildConnectionString(DBConnectionDTO connectionDTO)
        {
            // Build and return the connection string based on the DTO properties
            // This is just a simple example; in a real-world scenario, you would want to handle this more securely
            return $"Host={connectionDTO.HostName};Database={connectionDTO.DataBase};Username={connectionDTO.UserName};Password={connectionDTO.Password}";
        }

        public async Task<string> ReceiveHashFromPort(DBConnectionDTO connectionDTO)
        {
            DateTime now = DateTime.Now;
            string formatDateTime = now.ToString("yyyy-MM-dd HH:mm:ss.ffffff");

            //string timestampString = "2024-07-10 15:21:05.520756";
            string hashvalue = "78782622180614041f13cf01637f9f0898a0790914d2019428372700f91b0102000031cb9501382cba0d0a";
            int start = 8;
            int length = 16;

            string split = hashvalue.Remove(start, length);

            //DateTime timestamp = DateTime.ParseExact(
            //timestampString,
            //"yyyy-MM-dd HH:mm:ss.ffffff",
            //CultureInfo.InvariantCulture);

            DateTime timestamp = DateTime.ParseExact(
            formatDateTime,
            "yyyy-MM-dd HH:mm:ss.ffffff",
            CultureInfo.InvariantCulture);

            long unixTimeMilliseconds = new DateTimeOffset(timestamp).ToUnixTimeMilliseconds();
            byte[] byteArray = BitConverter.GetBytes(unixTimeMilliseconds);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteArray);
            }

            string hexValue = BitConverter.ToString(byteArray).Replace("-", string.Empty);

            string finalhashvalue = split.Insert(8, hexValue);

            Console.WriteLine($"Timestamp: {formatDateTime}");
            Console.WriteLine($"Unix Time (Milliseconds): {unixTimeMilliseconds}");

            Console.WriteLine($"Hash Value: {hashvalue}");
            Console.WriteLine($"Removed Hash Value: {split}");

            Console.WriteLine($"Hex Value: {hexValue}");
            Console.WriteLine($"FinalHash Value: {finalhashvalue}");

            return finalhashvalue;
        }



        public async Task<string> PortCommunication(DBConnectionDTO connectionDTO)
        {
            try
            {
                int portNumber = 1234; 
                string IPAddress = "192.168.1.100";

                string connectionString = BuildConnectionString(connectionDTO);
                // Create a new PostgreSQL connection
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Call ReceiveHashFromPort to get the final hash value
                    string finalHashValue = await ReceiveHashFromPort(connectionDTO);

                    return finalHashValue;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}
