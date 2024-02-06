using System.Collections.Generic;
using System.Threading.Tasks;
using ExcelSyncHub.Model.DTO;

namespace SchemaCraftHub.Service
{
    public interface ILogService
    {
        Task<LogDTO> GetLogByParentIdAsync(int logParentId);
        Task<LogDTO> GetLogByChildIdAsync(int logChildId);
        Task<List<LogDTO>> GetAllLogsAsync();
        Task<List<LogDTO>> GetLogsByUserIdAsync(int userId);
        Task<List<LogDTO>> GetLogsByEntityIdAsync(int entityId);
    }
}
