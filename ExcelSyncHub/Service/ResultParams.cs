
namespace ExcelSyncHub.Service
{
    public class ResultParams
    {
        public List<string> Filedatas { get; internal set; }
        public List<string> ErrorMessages { get; internal set; }
        public List<int> ErrorRowNumber { get; internal set; }
    }
}