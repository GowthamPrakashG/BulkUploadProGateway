﻿using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using Npgsql;
using System.Data.Common;
using Dapper;
using System.Data;
using Spire.Xls;

namespace ClientSchemaHub.Service
{
    public class TimescaleService : ITimescaleService
    {
        // Register the Npgsql provider
        public TimescaleService()
        {
            DbProviderFactories.RegisterFactory("Timescale", NpgsqlFactory.Instance);
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


        private string BuildConnectionString(DBConnectionDTO connectionDTO)
        {
            // Build and return the connection string based on the DTO properties
            // This is just a simple example; in a real-world scenario, you would want to handle this more securely
            return $"Host={connectionDTO.HostName};Database={connectionDTO.DataBase};Username={connectionDTO.UserName};Password={connectionDTO.Password}";
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
                            var dictionary = (IDictionary<string, object>)result;
                            return dictionary[primaryKeyColumnName];
                        }).ToList();

                        return idList;
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
    }
}
