namespace ClientSchemaHub.Models.DTO
{
    public class DBConnectionDTO
    {
        public string Provider { get; set; }
        public string? HostName { get; set; }
        public string? DataBase { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? AccessKey { get; set; }
        public string? SecretKey { get; set; }
        public string? Region { get; set; }
        public string? IPAddress { get;  set; }
        public int? PortNumber { get;  set; }
        public string? InfluxDbUrl { get;  set; }
        public string? InfluxDbToken { get;  set; }
        public string? InfluxDbOrg { get;  set; }
        public string? InfluxDbBucket { get;  set; }
        public string? Ec2Instance { get; set; }
        public string? Keyspace { get;  set; }
    }
}
