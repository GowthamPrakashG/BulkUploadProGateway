namespace SchemaCraftHub.Model.DTO
{
    public class ColumnDTO
    {
        public int Id { get; set; }
        public string ColumnName { get; set; }
        public string Datatype { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public int EntityId { get; set; }
        public int? ReferenceEntityID { get; set; }
        public int? ReferenceColumnID { get; set; }
        public int? Length { get; set; }
        public int? MinLength { set; get; }
        public int? MaxLength { set; get; }
        public int? MaxRange { set; get; }
        public int? MinRange { set; get; }
        public string? DateMinValue { set; get; }
        public string? DateMaxValue { set; get; }
        public string? Description { get; set; }
        public bool IsNullable { get; set; }
        public string? DefaultValue { get; set; }
        public string? True { get; set; }
        public string? False { get; set; }
    }
}
