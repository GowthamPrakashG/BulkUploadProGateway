using ClientSchemaHub.Models.DTO;
using SchemaCraftHub.Model.DTO;

namespace SchemaCraftHub.Service.IService
{
    public interface IEntitySchemaService
    {
        public Task<List<TableMetaDataDTO>> GetAllTablesAsync();
        public Task<TableMetaDataDTO> GetTableByIdAsync(int id);
        public Task<List<TableMetaDataDTO>> GetTablesByHostProviderDatabaseAsync(string hostName, string provider, string databaseName);
        public Task<TableMetaDataDTO> GetTableByHostProviderDatabaseTableNameAsync(string hostName, string provider, string databaseName, string tableName);
        public Task<List<ColumnMetaDataDTO>> GetAllColumnsAsync();
        public Task<ColumnMetaDataDTO> GetColumnByIdAsync(int id);
        public Task<ColumnMetaDataDTO> GetColumnByIdAndEntityIDAsync(int id, int entityId);
        public Task<List<ColumnMetaDataDTO>> GetColumnsByEntityIdAsync(int entityId);
        public Task<int> CreateTableAsync(TableMetaDataDTO tableDTO);
        public Task InsertColumnsAsync(List<ColumnMetaDataDTO> columns);
        public Task<Dictionary<string, List<TableDetailsDTO>>> GetClientSchema(APIResponse tabledetails1, DBConnectionDTO connectionDTO);
        public Task<ClientSchemaHub.Models.DTO.APIResponse> convertandcallcreatetablemodel(DBConnectionDTO connectionDTO, TableRequest tableRequest);

    }
}
