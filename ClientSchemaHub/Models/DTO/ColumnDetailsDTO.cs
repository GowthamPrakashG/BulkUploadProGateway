namespace ClientSchemaHub.Models.DTO
{
    public class ColumnDetailsDTO
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool HasForeignKey { get; set; }
        public string ReferencedTable { get; set; }
        public string ReferencedColumn { get; set; }
        public bool IsNullable { get; set; }
        public double? SomeNullableDouble { get; set; }

        // Add this property if it's needed
        public double? SomeNullableProperty { get; set; }
    }
}
