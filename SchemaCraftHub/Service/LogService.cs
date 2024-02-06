using DBUtilityHub.Data;
using DBUtilityHub.Models;
using ExcelSyncHub.Model.DTO;
using Microsoft.EntityFrameworkCore;

namespace SchemaCraftHub.Service
{
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LogDTO> GetLogByParentIdAsync(int logParentId)
        {
            var logParent = await _context.LogParents
                .FirstOrDefaultAsync(parent => parent.ID == logParentId);

            var logChild = await _context.LogChilds.Where(child => child.ParentID == logParentId).ToListAsync();

            if (logParent == null)
            {
                // Handle case where log with specified parent ID is not found
                return null;
            }

            var logDTO = new LogDTO
            {
                LogParentDTOs = logParent,
                ChildrenDTOs = logChild
            };

            return logDTO;
        }


        public async Task<LogDTO> GetLogByChildIdAsync(int logChildId)
        {
            var logChild = await _context.LogChilds
                .Include(child => child.Parent)
                .FirstOrDefaultAsync(child => child.ID == logChildId);

            if (logChild == null)
            {
                // Handle case where log with specified child ID is not found
                return null;
            }

            var logDTO = new LogDTO
            {
                LogParentDTOs = logChild.Parent,
                ChildrenDTOs = new List<LogChild> { logChild }
            };

            return logDTO;
        }

        // Implement similar methods for other criteria (Entity ID, User ID, and Get all logs) here.

        public async Task<List<LogDTO>> GetAllLogsAsync()
        {
            var logs = await _context.LogParents
                .ToListAsync();

            var logChild = await _context.LogChilds.ToListAsync();

            var logDTOs = logs.Select(logParent => new LogDTO
            {
                LogParentDTOs = logParent,
                ChildrenDTOs = logChild
            }).ToList();

            return logDTOs;
        }

        public async Task<List<LogDTO>> GetLogsByUserIdAsync(int userId)
        {
            var logs = await _context.LogParents
                .Where(log => log.User_Id == userId)
                .ToListAsync();

            var logChilds = await _context.LogChilds
                .Include(child => child.Parent)
                .Where(child => logs.Select(log => log.ID).Contains(child.ParentID))
                .ToListAsync();

            var logDTOs = logs.Select(logParent => new LogDTO
            {
                LogParentDTOs = logParent,
                ChildrenDTOs = logChilds.Where(child => child.ParentID == logParent.ID).ToList()
            }).ToList();

            return logDTOs;
        }

        public async Task<List<LogDTO>> GetLogsByEntityIdAsync(int entityId)
        {
            var logs = await _context.LogParents
                .Where(log => log.Entity_Id == entityId)
                .ToListAsync();

            var logChilds = await _context.LogChilds
                .Include(child => child.Parent)
                .Where(child => logs.Select(log => log.ID).Contains(child.ParentID))
                .ToListAsync();

            var logDTOs = logs.Select(logParent => new LogDTO
            {
                LogParentDTOs = logParent,
                ChildrenDTOs = logChilds.Where(child => child.ParentID == logParent.ID).ToList()
            }).ToList();

            return logDTOs;
        }

    }
}
