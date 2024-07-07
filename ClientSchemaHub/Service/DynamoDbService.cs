using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using System.Data.Common;

namespace ClientSchemaHub.Service
{
    public class DynamoDbService : IDynamoDbService
    {
        public  async Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsForAllTablesAsync(DBConnectionDTO connectionDTO)
        {
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(connectionDTO.Region);
            var credentials = new BasicAWSCredentials(connectionDTO.AccessKey, connectionDTO.SecretKey);
            var dynamoDbClient = new AmazonDynamoDBClient(credentials, regionEndpoint);

            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(connectionDTO.Provider);

                var request = new ListTablesRequest();
                var response = await dynamoDbClient.ListTablesAsync(request);


              //  List<string> tableNames = await GetTableNamesAsync(connection);


                if (response.TableNames.Count > 0)
                {
                    Console.WriteLine("Tables:");
                    foreach (var tableName in response.TableNames)
                    {
                        Console.WriteLine(tableName);
                    }

                    return response.TableNames;

                }
                else
                {
                    Console.WriteLine("No tables found.");
                    return new List<string>();
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }   
}

