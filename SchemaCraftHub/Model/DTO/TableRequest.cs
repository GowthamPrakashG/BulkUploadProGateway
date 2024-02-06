namespace SchemaCraftHub.Model.DTO
{
    public class TableRequest
    {
        public TableMetaDataDTO Table { get; set; }
        public List<ColumnDTO> Columns { get; set; }
    }
}
