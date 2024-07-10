using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ClientSchemaHub.Service
{
    public class DynamoDbService : IDynamoDbService
    {
        public async Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsForAllTablesAsync(DBConnectionDTO connectionDTO)
        {
            var dynamoDbClient = GetDynamoDbClient(connectionDTO);
            var tableNames = await GetTableNamesAsync(dynamoDbClient);
            Dictionary<string, List<TableDetailsDTO>> tableDetailsDictionary = new Dictionary<string, List<TableDetailsDTO>>();

            foreach (var tableName in tableNames)
            {
                var tableDetails = await GetTableDetailsAsync(dynamoDbClient, tableName);

                if (!tableDetailsDictionary.ContainsKey(tableName))
                {
                    tableDetailsDictionary[tableName] = new List<TableDetailsDTO>();
                }

                tableDetailsDictionary[tableName].Add(tableDetails);
            }

            return tableDetailsDictionary;
        }

        private AmazonDynamoDBClient GetDynamoDbClient(DBConnectionDTO connectionDTO)
        {
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(connectionDTO.Region);
            var credentials = new BasicAWSCredentials(connectionDTO.AccessKey, connectionDTO.SecretKey);
            return new AmazonDynamoDBClient(credentials, regionEndpoint);
        }

        private async Task<List<string>> GetTableNamesAsync(AmazonDynamoDBClient dynamoDbClient)
        {
            var tableNames = new List<string>();

            try
            {
                var request = new ListTablesRequest();
                var response = await dynamoDbClient.ListTablesAsync(request);

                tableNames.AddRange(response.TableNames);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

            return tableNames;
        }

        public async Task<List<string>> GetTableNamesAsync(DBConnectionDTO connectionDTO)
        {
            var dynamoDbClient = GetDynamoDbClient(connectionDTO);
            return await GetTableNamesAsync(dynamoDbClient);
        }

        private async Task<TableDetailsDTO> GetTableDetailsAsync(AmazonDynamoDBClient dynamoDbClient, string tableName)
        {
            var tableDetails = new TableDetailsDTO { TableName = tableName };

            try
            {
                var request = new DescribeTableRequest
                {
                    TableName = tableName
                };
                var response = await dynamoDbClient.DescribeTableAsync(request);
                var table = response.Table;

                tableDetails.Columns = table.AttributeDefinitions.Select(attr => new ColumnDetailsDTO
                {
                    ColumnName = attr.AttributeName,
                    DataType = attr.AttributeType
                }).ToList();
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
                    (
                        SELECT COUNT(1) > 0
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
                    (
                        SELECT ccu.table_name
                        FROM information_schema.key_column_usage kcu
                        JOIN information_schema.constraint_column_usage ccu ON ccu.constraint_name = kcu.constraint_name
                        JOIN information_schema.table_constraints tc ON tc.constraint_name = kcu.constraint_name
                        WHERE tc.table_name = @TableName
                        AND tc.constraint_type = 'FOREIGN KEY'
                        AND kcu.column_name = c.column_name
                    ) AS ReferencedTable,
                    (
                        SELECT ccu.column_name
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
                throw new ArgumentException(ex.Message);
            }

            return tableDetails;
        }

        public async Task<bool> IsTableExists(DBConnectionDTO connectionDTO, string tableName)
        {
            try
            {
                var dynamoDbClient = GetDynamoDbClient(connectionDTO);
                var request = new DescribeTableRequest
                {
                    TableName = tableName
                };
                var response = await dynamoDbClient.DescribeTableAsync(request);
                return response.Table != null;
            }
            catch (ResourceNotFoundException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if table exists: {ex.Message}");
                throw;
            }
        }

        // Get PrimaryKey records from the specific table
        public async Task<List<object>> GetPrimaryColumnDataAsync(DBConnectionDTO dBConnection, string tableName)
        {
            try
            {
                var dynamoDbClient = GetDynamoDbClient(dBConnection);

                // Describe table to get primary key information
                var describeTableRequest = new DescribeTableRequest
                {
                    TableName = tableName
                };
                var describeTableResponse = await dynamoDbClient.DescribeTableAsync(describeTableRequest);
                var primaryKeySchema = describeTableResponse.Table.KeySchema;

                // Identify the primary key attribute name
                var primaryKeyName = primaryKeySchema.FirstOrDefault(k => k.KeyType == "HASH")?.AttributeName;
                if (string.IsNullOrEmpty(primaryKeyName))
                {
                    throw new InvalidOperationException($"Table '{tableName}' does not have a primary key.");
                }

                // Scan the table to get primary key data
                var scanRequest = new ScanRequest
                {
                    TableName = tableName,
                    ProjectionExpression = primaryKeyName
                };
                var scanResponse = await dynamoDbClient.ScanAsync(scanRequest);
                var primaryKeys = scanResponse.Items.Select(item => item[primaryKeyName].S).ToList();

                // Convert primary keys to objects and return
                return primaryKeys.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<List<dynamic>> GetTabledata(DBConnectionDTO dBConnection, string tableName)
        {
            var dynamoDbClient = GetDynamoDbClient(dBConnection);

            // Scan the DynamoDB table to get all items
            var scanRequest = new ScanRequest
            {
                TableName = tableName
            };

            try
            {
                var scanResponse = await dynamoDbClient.ScanAsync(scanRequest);

                // Convert DynamoDB items to dynamic objects
                var rows = scanResponse.Items.Select(ConvertToDynamic).ToList();

                return rows;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error retrieving data from table {tableName}: {ex.Message}");
            }
        }

        // Helper method to convert DynamoDB Document to dynamic object
        private dynamic ConvertToDynamic(Dictionary<string, AttributeValue> item)
        {
            var expandoObj = new System.Dynamic.ExpandoObject() as IDictionary<string, Object>;

            foreach (var kvp in item)
            {
                expandoObj.Add(kvp.Key, GetObjectValue(kvp.Value));
            }

            return expandoObj;
        }

        // Helper method to convert DynamoDB AttributeValue to .NET object
        private object GetObjectValue(AttributeValue attributeValue)
        {
            if (attributeValue.S != null) return attributeValue.S;
            if (attributeValue.N != null) return attributeValue.N;
            if (attributeValue.SS.Count > 0) return attributeValue.SS;
            if (attributeValue.NS.Count > 0) return attributeValue.NS;
            if (attributeValue.M.Count > 0)
            {
                var dictionary = new Dictionary<string, object>();
                foreach (var kvp in attributeValue.M)
                {
                    dictionary.Add(kvp.Key, GetObjectValue(kvp.Value));
                }
                return dictionary;
            }
            if (attributeValue.L.Count > 0)
            {
                var list = new List<object>();
                foreach (var val in attributeValue.L)
                {
                    list.Add(GetObjectValue(val));
                }
                return list;
            }
            // Handle other types as needed

            return null; // Default case, should not happen with valid DynamoDB data
        }
    }

}

