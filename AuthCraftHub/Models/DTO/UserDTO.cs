using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DBUtilityHub.Models;

namespace AuthCraftHub.Models.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int RoleId { get; set; }
        public virtual RoleEntity Role { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Phonenumber { get; set; }

        public string Gender { get; set; }

        public DateOnly DOB { get; set; }

        public Boolean Status { get; set; } = true;

        public static explicit operator UserDTO(UserEntity data)
        {
            return new UserDTO { Id = data.Id, Name = data.Name, RoleId = data.RoleId, Role = data.Role, Email = data.Email, Password = data.Password, Phonenumber = data.Phonenumber, Gender = data.Gender, DOB = data.DOB, Status = data.Status };
        }

        public static implicit operator UserEntity(UserDTO data)
        {
            return new UserEntity { Id = data.Id, Name = data.Name, RoleId = data.RoleId, Role = data.Role, Email = data.Email, Password = data.Password, Phonenumber = data.Phonenumber, Gender = data.Gender, DOB = data.DOB, Status = data.Status };
        }
    }
}
