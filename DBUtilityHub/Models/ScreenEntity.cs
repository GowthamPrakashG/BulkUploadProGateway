using System.ComponentModel.DataAnnotations;

namespace DBUtilityHub.Models
{
    public class ScreenEntity
    {
        [Key]
        public int Id { get; set; }
        public string ScreenName { get; set; }
        public string RouteURL { get; set; }
    }
}
