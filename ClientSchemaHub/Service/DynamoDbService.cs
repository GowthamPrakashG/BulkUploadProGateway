using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientSchemaHub.Service
{
    public class DynamoDbService : IDynamoDbService
    {
        public async Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsForAllTablesAsync(DBConnectionDTO connectionDTO)
        {
            var dynamoDbClient = GetDynamoDbClient(connectionDTO);

            var tableDetails = await GetTableDetailsAsync(dynamoDbClient);

            return tableDetails;
        }

        private AmazonDynamoDBClient GetDynamoDbClient(DBConnectionDTO connectionDTO)
        {
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(connectionDTO.Region);
            var credentials = new BasicAWSCredentials(connectionDTO.AccessKey, connectionDTO.SecretKey);
            var dynamoDbClient = new AmazonDynamoDBClient(credentials, regionEndpoint);

            return dynamoDbClient;
        }

        private async Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsAsync(AmazonDynamoDBClient dynamoDbClient)
        {
            var tableDetails = new Dictionary<string, List<TableDetailsDTO>>();

            try
            {
                var request = new ListTablesRequest();
                var response = await dynamoDbClient.ListTablesAsync(request);

                if (response.TableNames.Count > 0)
                {
                    Console.WriteLine("Tables:");
                    foreach (var tableName in response.TableNames)
                    {
                        Console.WriteLine(tableName);

                        // Placeholder for retrieving table details
                        // Add table details logic here
                        tableDetails[tableName] = new List<TableDetailsDTO>();
                    }
                }
                else
                {
                    Console.WriteLine("No tables found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

            return tableDetails;
        }
    }
}
