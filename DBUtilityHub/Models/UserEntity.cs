using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DBUtilityHub.Models
{
    public class UserEntity : BaseModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Phone Number")]
        public string Phonenumber { get; set; }

        [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be 'Male', 'Female', or 'Other'.")]
        public string Gender { get; set; }


        [DataType(DataType.Date)]
        public DateOnly DOB { get; set; }

        public Boolean Status { get; set; } = true;
    }
}
