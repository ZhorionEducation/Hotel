using System.Collections.Generic;
namespace Hotel.Models
{
    public class DashboardViewModel
    {
        public List<int> UserRatings { get; set; }
        public Dictionary<string, int> MonthlyBookings { get; set; }
        public Dictionary<string, double> RoomOccupancy { get; set; }
    }
}
