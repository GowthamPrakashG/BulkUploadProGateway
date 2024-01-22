using DBUtilityHub.Models;

namespace ExcelSyncHub.Model.DTO
{
    public class LogDTO
    {
        public LogParent LogParentDTOs { get; set; }
        public List<LogChild> ChildrenDTOs { get; set; }
    }
}
