using Azure.Core.Extensions;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ClientSchemaHub.Service
{
    public class GeneralDatabaseService : IGeneralDatabaseService
    {
        private readonly IPostgreSQLService _postgreSQLService;
        private readonly IMySQLService _mySQLService;
        private readonly IMSSQLService _msSQLService;
        private readonly ITimescaleService _timescaleService;
        private readonly IDynamoDbService _dynamoDbService;
        private readonly IScyllaService _scyllaService;
        public GeneralDatabaseService(IPostgreSQLService postgreSQLService, IMySQLService mySQLService, IMSSQLService msSQLService, ITimescaleService timescaleService, IDynamoDbService dynamoDbService, IScyllaService scyllaService)
        {
            _postgreSQLService = postgreSQLService;
            _mySQLService = mySQLService;
            _msSQLService = msSQLService;
            _timescaleService = timescaleService;
            _dynamoDbService = dynamoDbService;
            _scyllaService = scyllaService;

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
                    case "Timescale": // Add MS SQL case
                        return await _timescaleService.GetTableDetailsForAllTablesAsync(connectionDTO);
                    case "Dynamo":
                        return await _dynamoDbService.GetTableDetailsForAllTablesAsync(connectionDTO);
                    case "Scylla":
                        return await _scyllaService.GetTableDetailsForAllTablesAsync(connectionDTO);
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
                    case "Timescale":
                        return await _timescaleService.GetTableNamesAsync(connectionDTO);
                    case "Scylla":
                        return await _scyllaService.GetTableNamesAsync(connectionDTO);
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
                        return await _mySQLService.GetTableDetailsAsync(connectionDTO, tableName);
                    case "MS SQL":
                        return await _msSQLService.GetTableDetailsAsync(connectionDTO, tableName);
                    case "Timescale":
                        return await _timescaleService.GetTableDetailsAsync(connectionDTO, tableName);
                    case "Scylla":
                        return await _scyllaService.GetTableDetailsAsync(connectionDTO, tableName);
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
                        return await _postgreSQLService.GetPrimaryColumnDataAsync(connectionDTO, tableName);
                    case "mysql": // Add the MySQL case
                        return await _mySQLService.GetPrimaryColumnDataAsync(connectionDTO, tableName);
                    // Add cases for other database providers
                    case "MS SQL":
                        return await _msSQLService.GetPrimaryColumnDataAsync(connectionDTO, tableName);
                    case "Timescale":
                        return await _timescaleService.GetPrimaryColumnDataAsync(connectionDTO, tableName);
                    case "Dynamo":
                        return await _dynamoDbService.GetPrimaryColumnDataAsync(connectionDTO, tableName);
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
                    case "Timescale":
                        return await _timescaleService.ConvertAndCallCreateTableModel(connectionDTO, query);
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
                        return await _msSQLService.Insertdata(connectionDTO, convertedDataList, booleancolumns, tablename);
                    case "Timescale":
                        return await _timescaleService.Insertdata(connectionDTO, convertedDataList, booleancolumns, tablename);
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
                    case "Timescale":
                        return await _timescaleService.IsTableExists(dBConnection, tableName);
                    case "Dynamo":
                        return await _dynamoDbService.IsTableExists(dBConnection, tableName);
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
                    case "Timescale":
                        return await _timescaleService.GetTabledata(dBConnection, tableName);
                    case "Dynamo":
                        return await _dynamoDbService.GetTabledata(dBConnection,tableName);
                    default:
                        throw new ArgumentException("Unsupported database provider");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }


        public async Task<string> ReceiveHashFromPort(DBConnectionDTO dBConnection)
        {
            try
            {
                switch (dBConnection.Provider)
                {
                    case "postgresql":
                        return await _postgreSQLService.ReceiveHashFromPort(dBConnection);
                    //case "mysql": // Add the MySQL case
                    //   return await _postgreSQLService.GetTabledata(dBConnection, tableName);
                    // Add cases for other database providers
                    //case "MS SQL":
                    //    return await _msSQLService.GetTabledata(dBConnection);
                    //case "Timescale":
                    //    return await _timescaleService.GetTabledata(dBConnection);
                    //case "Dynamo":
                    //    return await _dynamoDbService.GetTabledata(dBConnection);
                    default:
                        throw new ArgumentException("Unsupported database provider");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

        }


        public async Task<List<dynamic>> PortCommunication(DBConnectionDTO dBConnection, string tableName)
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
                    case "Timescale":
                        return await _timescaleService.GetTabledata(dBConnection, tableName);
                    case "Dynamo":
                        return await _dynamoDbService.GetTabledata(dBConnection, tableName);
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
