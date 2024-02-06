using ClientSchemaHub.Models.DTO;

namespace ClientSchemaHub.Service.IService
{
    public interface IGeneralDatabaseService
    {
        public Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsForAllTablesAsync(DBConnectionDTO connectionDTO);
        public Task<List<string>> GetTableNamesAsync(DBConnectionDTO connectionDTO);
        public Task<TableDetailsDTO> GetTableDetailsAsync(DBConnectionDTO dBConnection, string tableName);
        public Task<List<dynamic>> GetPrimaryColumnDataAsync(DBConnectionDTO dBConnection, string tableName);
        public Task<bool> CreateTable(DBConnectionDTO dBConnection, string query);
        public Task<bool> InsertdataGeneral(DBConnectionDTO? connectionDTO, List<Dictionary<string, string>>? convertedDataList, List<ColumnMetaDataDTO>? booleancolumns, string? tablename);
        public Task<bool> IsTableExists(DBConnectionDTO dBConnection, string tableName);
        public Task<List<dynamic>> GetTabledata(DBConnectionDTO dBConnection, string tableName);
    }
}
