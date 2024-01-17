using SchemaCraftHub.Model.DTO;

namespace SchemaCraftHub.Service.IService
{
    public interface IEntitySchemaService
    {
        public Task<List<TableMetaDataDTO>> GetAllTablesAsync();
        public Task<TableMetaDataDTO> GetTableByIdAsync(int id);
        public Task<List<TableMetaDataDTO>> GetTablesByHostProviderDatabaseAsync(string hostName, string provider, string databaseName);
        public Task<TableMetaDataDTO> GetTableByHostProviderDatabaseTableNameAsync(string hostName, string provider, string databaseName, string tableName);
        public Task<List<CloumnDTO>> GetAllColumnsAsync();
        public Task<CloumnDTO> GetColumnByIdAsync(int id);
        public Task<CloumnDTO> GetColumnByIdAndEntityIDAsync(int id, int entityId);
        public Task<List<CloumnDTO>> GetColumnsByEntityIdAsync(int entityId);
        public Task<int> CreateTableAsync(TableMetaDataDTO tableDTO);
        public Task InsertColumnsAsync(List<CloumnDTO> columns);
        public Task<Dictionary<string, List<ClientSchemaHub.Models.DTO.TableDetailsDTO>>> GetClientSchema(APIResponse tabledetails1, DBConnectionDTO connectionDTO);
        public Task<APIResponse> convertandcallcreatetablemodel(DBConnectionDTO connectionDTO, TableRequest tableRequest);
        public Task UpdateColumnsAsync(List<CloumnDTO> columns);

    }
}
