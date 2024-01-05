using DBUtilityHub.Models;

namespace AuthCraftHub.Models.DTO
{
    public class RoleDTO
    {
        public int Id { get; set; }
        public string RoleName { get; set; }

        public static explicit operator RoleDTO(RoleEntity data)
        {
            return new RoleDTO { Id = data.Id, RoleName = data.RoleName };
        }

        public static implicit operator RoleEntity(RoleDTO data)
        {
            return new RoleEntity { Id = data.Id, RoleName = data.RoleName };
        }
    }
}
