using DBUtilityHub.Models;

namespace AuthCraftHub.Models.DTO
{
    public class RoleScreenMappingDTO
    {
        
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int ScreenId { get; set; }
        public virtual RoleEntity Role { get; set; }
        public virtual ScreenEntity Screen { get; set; }

        public static explicit operator RoleScreenMappingDTO(RoleScreenMapping data)
        {
            return new RoleScreenMappingDTO { Id = data.Id, RoleId = data.RoleId, ScreenId = data.ScreenId, Role = data.Role,Screen = data.Screen };
        }

        public static implicit operator RoleScreenMapping(RoleScreenMappingDTO data)
        {
            return new RoleScreenMapping { Id = data.Id, RoleId = data.RoleId , ScreenId = data.ScreenId, Role = data.Role, Screen = data.Screen };
        }
    }
}