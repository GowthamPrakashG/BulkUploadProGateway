﻿namespace SchemaCraftHub.Model.DTO
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

    }
}
