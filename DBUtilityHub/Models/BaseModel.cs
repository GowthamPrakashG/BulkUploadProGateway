namespace DBUtilityHub.Models
{
    public class BaseModel
    {
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        public int UpdatedBy { get; set; }
    }

}

