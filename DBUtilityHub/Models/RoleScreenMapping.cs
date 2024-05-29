using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBUtilityHub.Models
{
    public class RoleScreenMapping
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public int ScreenId { get; set; }
        public virtual RoleEntity Role { get; set; }
        public virtual ScreenEntity Screen { get; set; }
         
    }
}
