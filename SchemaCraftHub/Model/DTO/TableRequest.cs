namespace SchemaCraftHub.Model.DTO
{
    public class TableRequest
    {
        public TableMetaDataDTO Table { get; set; }
        public List<ColumnMetaDataDTO> Columns { get; set; }
    }
}
