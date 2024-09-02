
namespace SchemaCraftHub.Model.DTO
{
    public class TableMetaDataDTO
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public string? HostName { get; set; }
        public string? DatabaseName { get; set; }
        public string Provider { get; set; }
        public string? AccessKey { get; set; } 
        public string? SecretKey { get; set; }
        public string? Region { get; set; }
        public string? InfluxDbOrg { get; set; }
        public string? InfluxDbToken { get; set; }
        public string? InfluxDbUrl { get; set; }
        public string? InfluxDbBucket { get; set; }

    }

}
