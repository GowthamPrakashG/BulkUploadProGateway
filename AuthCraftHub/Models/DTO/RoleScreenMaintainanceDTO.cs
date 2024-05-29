using DBUtilityHub.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthCraftHub.Models.DTO
{
    public class RoleScreenMaintainanceDTO
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public int ScreenId { get; set; }
        //public virtual RoleEntity Role { get; set; }
        //public virtual ScreenEntity Screen { get; set; }

        public static explicit operator RoleScreenMaintainanceDTO(RoleScreenMapping data)
        {
            return new RoleScreenMaintainanceDTO { Id = data.Id, RoleId = data.RoleId, ScreenId = data.ScreenId };//, Role = data.Role, Screen = data.Screen };
        }

        public static implicit operator RoleScreenMapping(RoleScreenMaintainanceDTO data)
        {
            return new RoleScreenMapping { Id = data.Id, RoleId = data.RoleId, ScreenId = data.ScreenId };//, Role = data.Role, Screen = data.Screen };
        }
    }
}

