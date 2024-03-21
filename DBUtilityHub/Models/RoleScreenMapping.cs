using System.ComponentModel.DataAnnotations;

namespace DBUtilityHub.Models
{
    public class RoleScreenMapping
    {
            [Key]
            public int Id { get; set; }
            public int RoleId { get; set; }
            public int ScreenId { get; set; }
    }
}
