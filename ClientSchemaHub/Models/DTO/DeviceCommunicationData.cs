namespace ClientSchemaHub.Models.DTO
{
    public class DeviceCommunicationData
    {
        public string DeviceId { get; set; }
        public string CommunicationData { get; set; }
        public DateTime Timestamp { get; set; }
        public string ProtocolHeader { get; set; }
        public string AuthToken { get; set; }
        public string ConfigParameters { get; set; }
        public string Checksum { get; set; }
        public string NetworkAddress { get; set; }
        public string SessionId { get; set; }
        public string EncryptionKey { get; set; }
    }
}
