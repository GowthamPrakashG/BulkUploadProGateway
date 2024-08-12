using ClientSchemaHub.Models.DTO;

namespace ClientSchemaHub.Service.IService
{
    public interface IScyllaService
    {
        Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsForAllTablesAsync(DBConnectionDTO connectionDTO);
    }
}