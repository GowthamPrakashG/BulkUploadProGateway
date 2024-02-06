using ExcelSyncHub.Model.DTO;

namespace ExcelSyncHub.Service
{
    internal class InsertDataRequest
    {
        public DBConnectionDTO ConnectionDTO { get; set; }
        public List<Dictionary<string, string>> ConvertedDataList { get; set; }
        public List<ColumnMetaDataDTO> BooleanColumns { get; set; }
        public string TableName { get; set; }
    }
}