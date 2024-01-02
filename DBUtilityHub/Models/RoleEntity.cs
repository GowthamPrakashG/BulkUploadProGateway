using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DBUtilityHub.Models
{
    public class RoleEntity 
    {
        [Key]
        public int Id { get; set; }
        public string RoleName { get; set; }
    }
}
