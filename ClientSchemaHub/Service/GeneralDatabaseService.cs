using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;

namespace ClientSchemaHub.Service
{
    public class GeneralDatabaseService : IGeneralDatabaseService
    {
        private readonly IPostgreSQLService _postgreSQLService;
        private readonly IMySQLService _mySQLService;
        private readonly IMSSQLService _msSQLService;
        private readonly IDynamoDbService _dynamoDbService;
        public GeneralDatabaseService(IPostgreSQLService postgreSQLService, IMySQLService mySQLService, IMSSQLService msSQLService, IDynamoDbService dynamoDbService)
        {
            _postgreSQLService = postgreSQLService;
            _mySQLService = mySQLService;
            _msSQLService = msSQLService;
            _dynamoDbService = dynamoDbService;
            
            // Initialize other database services
        }
        public async Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsForAllTablesAsync(DBConnectionDTO connectionDTO)
        {
            try
            {
                switch (connectionDTO.Provider)
                {
                    case "postgresql":
                        return await _postgreSQLService.GetTableDetailsForAllTablesAsync(connectionDTO);
                    case "mysql": // Add the MySQL case
                        return await _mySQLService.GetTableDetailsForAllTablesAsync(connectionDTO);
                    // Add cases for other database providers
                    case "mssql": // Add MS SQL case
                        return await _msSQLService.GetTableDetailsForAllTablesAsync(connectionDTO);
                    case "MS SQL": // Add MS SQL case
                        return await _msSQLService.GetTableDetailsForAllTablesAsync(connectionDTO);
                    case "Dynamo":
                        return await _dynamoDbService.GetTableDetailsForAllTablesAsync(connectionDTO);
                    default:
                        throw new ArgumentException("Unsupported database provider");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        public async Task<List<string>> GetTableNamesAsync(DBConnectionDTO connectionDTO)
        {
            try
            {
                switch (connectionDTO.Provider)
                {
                    case "postgresql":
                        return await _postgreSQLService.GetTableNamesAsync(connectionDTO);
                    case "mysql": // Add the MySQL case
                        return await _mySQLService.GetTableNamesAsync(connectionDTO);
                    // Add cases for other database providers
                    case "MS SQL":
                        return await _msSQLService.GetTableNamesAsync(connectionDTO);
                    default:
                        throw new ArgumentException("Unsupported database provider");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        public async Task<TableDetailsDTO> GetTableDetailsAsync(DBConnectionDTO connectionDTO, string tableName)
        {
            try
            {
                switch (connectionDTO.Provider)
                {
                    case "postgresql":
                        return await _postgreSQLService.GetTableDetailsAsync(connectionDTO, tableName);
                    case "mysql": // Add the MySQL case
                        return await _mySQLService.GetTableDetailsAsync(connectionDTO,tableName);
                    case "MS SQL":
                        return await _msSQLService.GetTableDetailsAsync(connectionDTO, tableName);
                    default:
                        throw new ArgumentException("Unsupported database provider");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        public async Task<List<dynamic>> GetPrimaryColumnDataAsync(DBConnectionDTO connectionDTO, string tableName)
        {
            try
            {
                switch (connectionDTO.Provider)
                {
                    case "postgresql":
                        return await _postgreSQLService.GetPrimaryColumnDataAsync(connectionDTO , tableName);
                    case "mysql": // Add the MySQL case
                        return await _mySQLService.GetPrimaryColumnDataAsync(connectionDTO, tableName);
                    // Add cases for other database providers
                    case "MS SQL":
                        return await _msSQLService.GetPrimaryColumnDataAsync(connectionDTO, tableName);
                    default:
                        throw new ArgumentException("Unsupported database provider");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<bool> CreateTable(DBConnectionDTO connectionDTO, string query)
        {
            try
            {
                switch (connectionDTO.Provider)
                {
                    case "postgresql":
                        return await _postgreSQLService.ConvertAndCallCreateTableModel(connectionDTO, query);
                    case "mysql": // Add the MySQL case
                        return await _mySQLService.ConvertAndCallCreateTableModel(connectionDTO, query);
                    // Add cases for other database providers
                    case "MS SQL":
                        return await _msSQLService.ConvertAndCallCreateTableModel(connectionDTO, query);
                    default:
                        throw new ArgumentException("Unsupported database provider");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<bool> InsertdataGeneral(DBConnectionDTO? connectionDTO, List<Dictionary<string, string>>? convertedDataList, List<ColumnMetaDataDTO>? booleancolumns, string? tablename)
        {
            try
            {
                switch (connectionDTO.Provider)
                {
                    case "postgresql":
                        return await _postgreSQLService.Insertdata(connectionDTO, convertedDataList, booleancolumns, tablename);
                    case "mysql": // Add the MySQL case
                        return await _postgreSQLService.Insertdata(connectionDTO, convertedDataList, booleancolumns, tablename);//Change
                    case "MS SQL":
                        return await _msSQLService.Insertdata(connectionDTO, convertedDataList, booleancolumns,tablename);
                    // Add cases for other database providers
                    default:
                        throw new ArgumentException("Unsupported database provider");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<bool> IsTableExists(DBConnectionDTO dBConnection, string tableName)
        {
            try
            {
                switch (dBConnection.Provider)
                {
                    case "postgresql":
                        return await _postgreSQLService.IsTableExists(dBConnection, tableName);
                    case "mysql": // Add the MySQL case
                        return await _postgreSQLService.IsTableExists(dBConnection, tableName);//Changes
                    // Add cases for other database providers
                    case "MS SQL":
                        return await _msSQLService.IsTableExists(dBConnection, tableName);
                    default:
                        throw new ArgumentException("Unsupported database provider");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<List<dynamic>> GetTabledata(DBConnectionDTO dBConnection, string tableName)
        {
            try
            {
                switch (dBConnection.Provider)
                {
                    case "postgresql":
                        return await _postgreSQLService.GetTabledata(dBConnection, tableName);
                     //case "mysql": // Add the MySQL case
                     //   return await _postgreSQLService.GetTabledata(dBConnection, tableName);
                    // Add cases for other database providers
                    case "MS SQL":
                        return await _msSQLService.GetTabledata(dBConnection, tableName);

                    //case "Dynamo":
                    //    return await _dynamoDbService.GetTabledata(dBConnection,tableName);
                    default:
                        throw new ArgumentException("Unsupported database provider");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }

}
