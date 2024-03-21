using DBUtilityHub.Models;

namespace AuthCraftHub.Models.DTO
{
    public class ScreenDTO
    {
        public int Id { get; set; }
        public string ScreenName { get; set; }
        public string RouteURL { get; set; }

        public static explicit operator ScreenDTO(ScreenEntity data)
        {
            return new ScreenDTO { Id = data.Id, ScreenName = data.ScreenName, RouteURL = data.RouteURL };
        }

        public static implicit operator ScreenEntity(ScreenDTO data)
        {
            return new ScreenEntity { Id = data.Id, ScreenName = data.ScreenName, RouteURL = data.RouteURL };
        }
    }
}
