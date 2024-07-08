using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
