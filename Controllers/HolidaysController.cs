using Microsoft.AspNetCore.Mvc;
using CrumbCount2.Models;
using CrumbCount2.Services;

namespace CrumbCount2.Controllers
{
    public class HolidaysController : Controller
    {
        private readonly IHolidayService _holidayService;

        public HolidaysController(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        public async Task<IActionResult> Index(
            CancellationToken cancellationToken)
        {
            int year = DateTime.Now.Year;

            ViewBag.Year = year;

            try
            {
                var holidays = await _holidayService.GetHolidaysAsync(
                    year, cancellationToken);

                return View(holidays);
            }
            catch (Exception ex) when (
                ex is HttpRequestException ||
                ex is System.Text.Json.JsonException ||
                ex is TaskCanceledException)
            {
                ViewBag.Error =
                    "Could not load the holidays from the outside website. Try again later.";

                return View(new List<Holiday>());
            }
        }
    }
}
