using System.ComponentModel.DataAnnotations;

namespace DBUtilityHub.Models
{

    public class TableMetaDataEntity : BaseModel
    {
        [Key]
        public int Id { get; set; }
        public string EntityName { get; set; }
        public string? HostName { get; set; }
        public string? DatabaseName { get; set; }
        public string Provider { get; set; }
        public string? AccessKey { get; set; }
        public string? SecretKey { get; set; }
        public string? Region { get; set; }
        public string? IPAddress { get; set; }
        public int? PortNumber { get; set; }
        public string? Ec2Instance { get; set; }
        public string? Keyspace { get; set; }
    }

}
