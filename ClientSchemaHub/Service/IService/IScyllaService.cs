using ClientSchemaHub.Models.DTO;

namespace ClientSchemaHub.Service.IService
{
    public interface IScyllaService
    {
        Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsForAllTablesAsync(DBConnectionDTO connectionDTO);
        public Task<List<string>> GetTableNamesAsync(DBConnectionDTO connectionDTO);
        public Task<TableDetailsDTO> GetTableDetailsAsync(DBConnectionDTO dBConnection, string tableName);
        public Task<List<dynamic>> GetTableData(DBConnectionDTO dBConnection, string tableName);
        public Task<bool> IsTableExists(DBConnectionDTO connectionDTO, string tableName);
        public Task<List<object>> GetPrimaryColumnDataAsync(DBConnectionDTO connectionDTO, string tableName);
    }
}