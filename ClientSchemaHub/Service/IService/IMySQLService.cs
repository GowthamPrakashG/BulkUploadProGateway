using ClientSchemaHub.Models.DTO;

namespace ClientSchemaHub.Service.IService
{
    public interface IMySQLService
    {
        public Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsForAllTablesAsync(DBConnectionDTO connectionDTO);
        public Task<List<string>> GetTableNamesAsync(DBConnectionDTO dBConnection);

        public Task<TableDetailsDTO> GetTableDetailsAsync(DBConnectionDTO dBConnection, string tableName);

        public Task<List<object>> GetPrimaryColumnDataAsync(DBConnectionDTO dBConnection, string tableName);

        public Task<bool> ConvertAndCallCreateTableModel(DBConnectionDTO connectionDTO, string createQuery);
    }
}
