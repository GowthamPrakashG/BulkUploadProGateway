using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using InfluxDB.Client;
namespace ClientSchemaHub.Service
{
    public class InfluxDbService : IInfluxDbService
    {
        public async Task<bool> IsTableExists(DBConnectionDTO dBConnection, string tableName)
        {
            try
            {
                var options = new InfluxDBClientOptions.Builder()
                    .Url(dBConnection.InfluxDbUrl)
                    .AuthenticateToken(dBConnection.InfluxDbToken.ToCharArray())
                    .Org(dBConnection.InfluxDbOrg)
                    .Build();

                using (var influxDbClient = new InfluxDBClient(options))
                {

                    var queryApi = influxDbClient.GetQueryApi();

                    // Query to check if the measurement exists in the specified bucket
                    string query = $"import \"influxdata/influxdb/schema\"\n" +
                                   $"schema.measurements(bucket: \"{dBConnection.InfluxDbBucket}\")";

                    var fluxTables = await queryApi.QueryAsync(query);

                    bool measurementExists = fluxTables
                        .SelectMany(table => table.Records)
                        .Any(record => record.GetValue().ToString() == tableName);

                    return measurementExists;

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while checking the bucket existence: {ex.Message}", ex);
            }
        }

        public async Task<List<dynamic>> GetTabledata(DBConnectionDTO dBConnection, string tableName)
        {
            try
            {
                var options = new InfluxDBClientOptions.Builder()
                    .Url(dBConnection.InfluxDbUrl)
                    .AuthenticateToken(dBConnection.InfluxDbToken.ToCharArray())
                    .Org(dBConnection.InfluxDbOrg)
                    .Build();

                using (var influxDbClient = new InfluxDBClient(options))
                {
                    var queryApi = influxDbClient.GetQueryApi();

                    // Flux query to retrieve all data from the specified measurement
                    string query = $"from(bucket: \"{dBConnection.InfluxDbBucket}\") " +
                                   $"|> range(start: 0) " +  // Adjust time range as needed
                                   $"|> filter(fn: (r) => r._measurement == \"{tableName}\")";

                    var fluxTables = await queryApi.QueryAsync(query);

                    var data = fluxTables.SelectMany(table => table.Records.Select(record => new
                    {
                        Time = record.GetTime(),
                        Measurement = record.GetMeasurement(),
                        Field = record.GetField(),
                        Value = record.GetValue()
                    })).ToList<dynamic>();

                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving data: {ex.Message}", ex);
            }
        }


        public async Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsForAllTablesAsync(DBConnectionDTO connectionDTO)
        {
            var influxDbClient = GetInfluxDbClient(connectionDTO);
            var bucketNames = await GetBucketNamesAsync(influxDbClient); // equivalent to table names in InfluxDB
            Dictionary<string, List<TableDetailsDTO>> tableDetailsDictionary = new Dictionary<string, List<TableDetailsDTO>>();
            foreach (var bucketName in bucketNames)
            {
                var tableDetails = await GetTableDetailsAsync(connectionDTO, bucketName);

                if (!tableDetailsDictionary.ContainsKey(bucketName))
                {
                    tableDetailsDictionary[bucketName] = new List<TableDetailsDTO>();
                }

                tableDetailsDictionary[bucketName].Add(tableDetails);
            }
            return tableDetailsDictionary;
        }
        public async Task<TableDetailsDTO> GetTableDetailsAsync(DBConnectionDTO dBConnection, string tableName)
        {
            var influxDbClient = GetInfluxDbClient(dBConnection);

            var tableDetails = new TableDetailsDTO { TableName = tableName };

            try
            {
                var query = $"from(bucket: \"{dBConnection.InfluxDbBucket}\") |> range(start: -1h) |> limit(n:1)";
                var fluxTables = await influxDbClient.GetQueryApi().QueryAsync(query, dBConnection.InfluxDbOrg);

                if (fluxTables.Count > 0)
                {
                    var fluxTable = fluxTables[0];
                    tableDetails.Columns = fluxTable.Columns.Select(col => new ColumnDetailsDTO
                    {
                        ColumnName = col.Label,
                        DataType = col.DataType
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching details for bucket {dBConnection.InfluxDbBucket}: {ex.Message}");
                throw;
            }

            return tableDetails;
        }

        public InfluxDBClient GetInfluxDbClient(DBConnectionDTO connectionDTO)
        {
            //var influxDbClient = InfluxDBClientFactory.Create(connectionDTO.InfluxDbUrl, connectionDTO.InfluxDbToken, connectionDTO.InfluxDbOrg.ToCharArray());

            var options = new InfluxDBClientOptions.Builder()
                        .Url(connectionDTO.InfluxDbUrl)
                        .AuthenticateToken(connectionDTO.InfluxDbToken.ToCharArray())
                        .Org(connectionDTO.InfluxDbOrg)
                        .Build();

            var influxDbClient = new InfluxDBClient(options);
            return influxDbClient;
        }

        public async Task<List<string>> GetBucketNamesAsync(InfluxDBClient influxDbClient)
        {
            var bucketsApi = influxDbClient.GetBucketsApi();
            var buckets = await bucketsApi.FindBucketsAsync();
            return buckets.Select(bucket => bucket.Name).ToList();
        }


        public async Task<List<object>> GetPrimaryColumnDataAsync(DBConnectionDTO dBConnection, string tableName)
        {
            try
            {
                var options = new InfluxDBClientOptions.Builder()
                    .Url(dBConnection.InfluxDbUrl)
                    .AuthenticateToken(dBConnection.InfluxDbToken.ToCharArray())
                    .Org(dBConnection.InfluxDbOrg)
                    .Build();

                using (var influxDbClient = new InfluxDBClient(options))
                {
                    var queryApi = influxDbClient.GetQueryApi();

                    string query = $@"
                                   from(bucket: ""{dBConnection.InfluxDbBucket}"") |> range(start: 0) // Adjust the time range as needed |> filter(fn: (r) => r._measurement == ""{tableName}"") |> keep(columns: [""_field"", ""_value"", ""_time""]) |> group(columns: [""_field""]) |> sort(columns: [""_time""])";

                    var fluxTables = await queryApi.QueryAsync(query);

                    // Extract the first field's values as the "primary column" data
                    var data = fluxTables
                .SelectMany(table => table.Records)
                .GroupBy(record => record.GetField())
                .SelectMany(group => group.Select(record => record.GetValue()))
                .ToList();

                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"An error occurred while retrieving data: {ex.Message}");
            }
        }

    }
}
