using ExcelSyncHub.Model.DTO;

namespace ExcelSyncHub.Models.DTO
{
    public class InsertDataRequest
    {
        public DBConnectionDTO ConnectionDTO { get; set; }
        public List<Dictionary<string, string>> ConvertedDataList { get; set; }
        public List<ColumnMetaDataDTO> BooleanColumns { get; set; }
        public string TableName { get; set; }
    }

}
