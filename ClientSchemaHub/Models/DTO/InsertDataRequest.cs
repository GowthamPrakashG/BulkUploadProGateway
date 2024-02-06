using ClientSchemaHub.Models.DTO;

public class InsertDataRequest
{
    public string? ConnectionDTO { get; set; }
    public string? ConvertedDataList { get; set; }
    public string? BooleanColumns { get; set; }
    public string? TableName { get; set; }
}
