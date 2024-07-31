using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using Dapper;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClientSchemaHub.Service
{
    public class AzureService : IAzureService
    {
        private CosmosClient GetCosmosDbClient(DBConnectionDTO connectionDTO)
        {
            return new CosmosClient(connectionDTO.AccountEndpoint, connectionDTO.AccountKey);
        }

        public async Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsForAllTablesAsync(DBConnectionDTO connectionDTO)
        {
            // Create a new instance of the CosmosClient using the GetCosmosDbClient method
            var cosmosClient = GetCosmosDbClient(connectionDTO);
            var database = cosmosClient.GetDatabase(connectionDTO.DataBase);

            // Get all container names (in Cosmos DB, these are equivalent to table names)
            var containerNames = new List<string>();
            var containerIterator = database.GetContainerQueryIterator<ContainerProperties>();

            while (containerIterator.HasMoreResults)
            {
                var containers = await containerIterator.ReadNextAsync();
                containerNames.AddRange(containers.Select(c => c.Id));
            }

            Dictionary<string, List<TableDetailsDTO>> tableDetailsDictionary = new Dictionary<string, List<TableDetailsDTO>>();

            foreach (var containerName in containerNames)
            {
                // Get table details (in Cosmos DB, this is a query on the container)
                var tableDetails = await GetTableDetailsAsync(cosmosClient, connectionDTO.DataBase, containerName);

                if (!tableDetailsDictionary.ContainsKey(containerName))
                {
                    tableDetailsDictionary[containerName] = new List<TableDetailsDTO>();
                }

                tableDetailsDictionary[containerName].Add(tableDetails);
            }

            return tableDetailsDictionary;
        }

        // Modified GetTableDetailsAsync method for Cosmos DB
        private async Task<TableDetailsDTO> GetTableDetailsAsync(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            var database = cosmosClient.GetDatabase(databaseName);
            var container = database.GetContainer(containerName);

            // Query the container to get table details
            var query = container.GetItemQueryIterator<TableDetailsDTO>(new QueryDefinition("SELECT * FROM c"));
            var results = await query.ReadNextAsync();

            return results.FirstOrDefault();
        }

        //Is table Exists
        public async Task<bool> IsTableExists(DBConnectionDTO connectionDTO, string tableName)
        {
            try
            {
                // Create a new instance of the CosmosClient using the GetCosmosDbClient method
                var cosmosClient = GetCosmosDbClient(connectionDTO);
                var database = cosmosClient.GetDatabase(connectionDTO.DataBase);
                var container = database.GetContainer(tableName);
                var response = await container.ReadContainerAsync();
                return response != null;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if table exists: {ex.Message}");
                throw;
            }
        }

        public Task<List<object>> GetPrimaryColumnDataAsync(DBConnectionDTO connectionDTO, string tableName)
        {
            throw new NotImplementedException();
        }
    }
}