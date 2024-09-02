using ClientSchemaHub.Models.DTO;

namespace ClientSchemaHub.Service.IService
{
    public interface IInfluxDbService
    {
        public Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsForAllTablesAsync(DBConnectionDTO dBConnection);
       // public Task<List<string>> GetTableNamesAsync(DBConnectionDTO connectionDTO);
        public Task<TableDetailsDTO> GetTableDetailsAsync(DBConnectionDTO dBConnection, string tableName);
        public Task<bool> IsTableExists(DBConnectionDTO dBConnection, string tableName);
        public Task<List<dynamic>> GetTabledata(DBConnectionDTO dBConnection, string tableName);
        public Task<List<object>> GetPrimaryColumnDataAsync(DBConnectionDTO dBConnection, string tableName);

    }
}
