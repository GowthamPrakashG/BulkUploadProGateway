using Amazon.DynamoDBv2;

namespace ClientSchemaHub.Models.DTO
{
    public class TableDetailsDTO
    {
        public string TableName { get; set; }
        public List<ColumnDetailsDTO> Columns { get; set; }
        public string AttributeName { get; internal set; }
        public ScalarAttributeType AttributeType { get; internal set; }
    }
}
