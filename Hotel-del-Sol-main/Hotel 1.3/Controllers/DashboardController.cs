using Microsoft.AspNetCore.Mvc;
using Hotel.Models;
using System.Collections.Generic;

namespace Hotel.Controllers
{
    [AuthorizePermission("Dashboard")]
    public class DashboardController : Controller
    {
        [AuthorizePermission("Dashboard")]
        public IActionResult Index()
        {
            var viewModel = new DashboardViewModel
            {
                UserRatings = new List<int> { 5, 4, 3, 5, 2 },
                MonthlyBookings = new Dictionary<string, int>
                {
                    {"Enero", 50}, {"Febrero", 60}, {"Marzo", 70},
                    {"Abril", 65}, {"Mayo", 80}, {"Junio", 90}
                },
                RoomOccupancy = new Dictionary<string, double>
                {
                    {"Simple", 70}, {"Doble", 85}, {"Suite", 60}
                }
            };

            return View(viewModel);
        }
    }
}
