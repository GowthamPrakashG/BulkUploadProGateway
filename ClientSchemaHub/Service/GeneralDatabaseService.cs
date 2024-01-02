using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;

namespace ClientSchemaHub.Service
{
    public class GeneralDatabaseService : IGeneralDatabaseService
    {
        private readonly IPostgreSQLService _postgreSQLService;
        private readonly IMySQLService _mySQLService;

        public GeneralDatabaseService(IPostgreSQLService postgreSQLService, IMySQLService mySQLService)
        {
            _postgreSQLService = postgreSQLService;
            _mySQLService = mySQLService;
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
