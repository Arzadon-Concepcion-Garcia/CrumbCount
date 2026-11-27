
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CrumbCount2.Models;
using CrumbCount2.Models.ViewModels;
using CrumbCount2.Services;
using System.Text;

namespace CrumbCount2.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GeminiService _geminiService;
        private readonly IHolidayService _holidayService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(
            ApplicationDbContext context,
            GeminiService geminiService,
            IHolidayService holidayService,
            ILogger<ReportsController> logger)
        {
            _context = context;
            _geminiService = geminiService;
            _holidayService = holidayService;
            _logger = logger;
        }

        // Administrator and Staff can view reports
        [Authorize(Roles = "Administrator,Staff")]
        public async Task<IActionResult> Index(string period = "Today")
        {
            var report = await GetWasteReport(period);

            return View(report);
        }

        // Administrator and Staff can generate AI insights
        [Authorize(Roles = "Administrator,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateInsights(string period = "Today")
        {
            var report = await GetWasteReport(period);

            if (report.TotalWasteRecords == 0)
            {
                report.AiInsights =
                    "No waste records are available for AI analysis.";

                return View("Index", report);
            }

            try
            {
                string reportData = CreateReportData(report);

                string holidayData;

                try
                {
                    var holidays = await _holidayService
                        .GetHolidaysAsync(DateTime.Now.Year);

                    StringBuilder holidayBuilder = new StringBuilder();

                    holidayBuilder.AppendLine(
                        "Philippine Public Holidays:");

                    foreach (var holiday in holidays)
                    {
                        holidayBuilder.AppendLine(
                            $"{holiday.Date:yyyy-MM-dd} - {holiday.LocalName} ({holiday.Name})");
                    }

                    holidayData = holidayBuilder.ToString();
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "HOLIDAY API ERROR while retrieving holidays.");

                    holidayData =
                        "Holiday information is unavailable. Do not assume holiday dates.";
                }

                report.AiInsights = await _geminiService
                    .GenerateWasteInsights(reportData, holidayData);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "GEMINI ERROR while generating waste insights.");

                report.AiInsights =
                    "AI insights are currently unavailable. Please try again later.";
            }

            return View("Index", report);
        }

        private async Task<WasteReportViewModel> GetWasteReport(string period)
        {
            DateTime today = DateTime.Today;

            DateTime startDate;
            DateTime endDate;

            // Determine the selected date range
            switch (period?.ToLower())
            {
                case "weekly":
                    int daysSinceMonday =
                        ((int)today.DayOfWeek + 6) % 7;

                    startDate = today.AddDays(-daysSinceMonday);
                    endDate = startDate.AddDays(6);
                    period = "Weekly";
                    break;

                case "monthly":
                    startDate = new DateTime(
                        today.Year, today.Month, 1);

                    endDate = startDate.AddMonths(1).AddDays(-1);
                    period = "Monthly";
                    break;

                default:
                    startDate = today;
                    endDate = today;
                    period = "Today";
                    break;
            }

            // Retrieve waste records within the selected date range
            var wasteRecords = await _context.WasteRecordings
                .Where(w =>
                    w.DateRecorded >= startDate &&
                    w.DateRecorded < endDate.AddDays(1))
                .Include(w => w.Product)
                .ThenInclude(p => p!.Category)
                .ToListAsync();

            var report = new WasteReportViewModel();

            report.SelectedPeriod = period;
            report.StartDate = startDate;
            report.EndDate = endDate;

            // Store individual waste records
            report.WasteRecords = wasteRecords;

            report.TotalWasteRecords = wasteRecords.Count;

            report.TotalQuantityWasted = wasteRecords
                .Sum(w => w.QuantityWasted);

            report.TotalWasteCost = wasteRecords
                .Sum(w => w.QuantityWasted * (w.Product?.UnitCost ?? 0));

            // Group waste by product
            report.ProductSummaries = wasteRecords
                .GroupBy(w => w.Product?.ProductName ?? "Unknown Product")
                .Select(g => new ProductWasteSummary
                {
                    ProductName = g.Key,
                    QuantityWasted = g.Sum(w => w.QuantityWasted),
                    WasteCost = g.Sum(w =>
                        w.QuantityWasted * (w.Product?.UnitCost ?? 0))
                })
                .ToList();

            // Group waste by category
            report.CategorySummaries = wasteRecords
                .GroupBy(w => w.Product?.Category?.CategoryName ?? "Uncategorized")
                .Select(g => new CategoryWasteSummary
                {
                    CategoryName = g.Key,
                    WasteCost = g.Sum(w =>
                        w.QuantityWasted * (w.Product?.UnitCost ?? 0))
                })
                .ToList();

            // Group waste by reason
            report.ReasonSummaries = wasteRecords
                .GroupBy(w => w.ReasonForWaste)
                .Select(g => new ReasonWasteSummary
                {
                    Reason = g.Key,
                    QuantityWasted = g.Sum(w => w.QuantityWasted),
                    WasteCost = g.Sum(w =>
                        w.QuantityWasted * (w.Product?.UnitCost ?? 0))
                })
                .ToList();

            return report;
        }

        private string CreateReportData(WasteReportViewModel report)
        {
            StringBuilder reportData = new StringBuilder();

            reportData.AppendLine("WASTE REPORT:");
            reportData.AppendLine(
                $"Selected Period: {report.SelectedPeriod}");
            reportData.AppendLine(
                $"Date Range: {report.StartDate:yyyy-MM-dd} to {report.EndDate:yyyy-MM-dd}");

            reportData.AppendLine("\nOVERALL SUMMARY:");
            reportData.AppendLine(
                $"Total Waste Records: {report.TotalWasteRecords}");
            reportData.AppendLine(
                $"Total Quantity Wasted: {report.TotalQuantityWasted}");
            reportData.AppendLine(
                $"Total Waste Cost: ₱{report.TotalWasteCost:N2}");

            reportData.AppendLine("\nINDIVIDUAL WASTE RECORDS:");

            foreach (var waste in report.WasteRecords)
            {
                reportData.AppendLine(
                    $"Date: {waste.DateRecorded:yyyy-MM-dd} | " +
                    $"Product: {waste.Product?.ProductName ?? "Unknown Product"} | " +
                    $"Quantity: {waste.QuantityWasted} | " +
                    $"Reason: {waste.ReasonForWaste} | " +
                    $"Estimated Cost: ₱{waste.EstimatedWasteCost:N2}");
            }

            reportData.AppendLine("\nWASTE BY PRODUCT:");

            foreach (var item in report.ProductSummaries)
            {
                reportData.AppendLine(
                    $"{item.ProductName}: Quantity Wasted = {item.QuantityWasted}, " +
                    $"Waste Cost = ₱{item.WasteCost:N2}");
            }

            reportData.AppendLine("\nWASTE BY CATEGORY:");

            foreach (var item in report.CategorySummaries)
            {
                reportData.AppendLine(
                    $"{item.CategoryName}: Waste Cost = ₱{item.WasteCost:N2}");
            }

            reportData.AppendLine("\nWASTE BY REASON:");

            foreach (var item in report.ReasonSummaries)
            {
                reportData.AppendLine(
                    $"{item.Reason}: Quantity Wasted = {item.QuantityWasted}, " +
                    $"Waste Cost = ₱{item.WasteCost:N2}");
            }

            return reportData.ToString();
        }
    }
}
