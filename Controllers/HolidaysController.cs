
using Microsoft.AspNetCore.Mvc;
using CrumbCount2.Models;

namespace CrumbCount2.Controllers
{
    // Gets the Philippine public holidays from an OUTSIDE website (an API) and shows them
    public class HolidaysController : Controller
    {
        private readonly HttpClient _http;

        // HttpClient is the tool that lets our site call other websites
        public HolidaysController(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            int year = DateTime.Now.Year;

            // The outside website's address (free, no key needed). "PH" = Philippines
            var url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/PH";

            ViewBag.Year = year;

            try
            {
                // Ask the website for its data and turn the JSON into a list of Holidays
                var holidays = await _http.GetFromJsonAsync<List<Holiday>>(url) ?? new List<Holiday>();
                return View(holidays);
            }
            catch (Exception)
            {
                // If the website is down or there's no internet, show a message instead of crashing
                ViewBag.Error = "Could not load the holidays from the outside website. Try again later.";
                return View(new List<Holiday>());
            }
        }
    }
}
