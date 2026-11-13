
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public ReportsController(
            ApplicationDbContext context,
            GeminiService geminiService)
        {
            _context = context;
            _geminiService = geminiService;
        }

        public async Task<IActionResult> Index()
        {
            var report = await GetWasteReport();

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateInsights()
        {
            var report = await GetWasteReport();

            if (report.TotalWasteRecords == 0)
            {
                report.AiInsights =
                    "No waste records are available for AI analysis.";

                return View("Index", report);
            }

            try
            {
                string reportData = CreateReportData(report);

                report.AiInsights = await _geminiService
                    .GenerateWasteInsights(reportData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GEMINI ERROR:");
                Console.WriteLine(ex.ToString());

                report.AiInsights =
                    "AI insights are currently unavailable. Please try again later.";
            }

            return View("Index", report);
        }

        private async Task<WasteReportViewModel> GetWasteReport()
        {
            var wasteRecords = await _context.WasteRecordings
                .Include(w => w.Product)
                .ThenInclude(p => p!.Category)
                .ToListAsync();

            var report = new WasteReportViewModel();

            report.TotalWasteRecords = wasteRecords.Count;

            report.TotalQuantityWasted = wasteRecords
                .Sum(w => w.QuantityWasted);

            report.TotalWasteCost = wasteRecords
                .Sum(w => w.QuantityWasted * (w.Product?.UnitCost ?? 0));

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

            report.CategorySummaries = wasteRecords
                .GroupBy(w => w.Product?.Category?.CategoryName ?? "Uncategorized")
                .Select(g => new CategoryWasteSummary
                {
                    CategoryName = g.Key,
                    WasteCost = g.Sum(w =>
                        w.QuantityWasted * (w.Product?.UnitCost ?? 0))
                })
                .ToList();

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

    reportData.AppendLine("Overall Summary:");
    reportData.AppendLine(
        $"Total Waste Records: {report.TotalWasteRecords}");
    reportData.AppendLine(
        $"Total Quantity Wasted: {report.TotalQuantityWasted}");
    reportData.AppendLine(
        $"Total Waste Cost: ₱{report.TotalWasteCost:N2}");

    reportData.AppendLine("\nWaste by Product:");

    foreach (var item in report.ProductSummaries)
    {
        reportData.AppendLine(
            $"{item.ProductName}: Quantity Wasted = {item.QuantityWasted}, Waste Cost = ₱{item.WasteCost:N2}");
    }

    reportData.AppendLine("\nWaste by Category:");

    foreach (var item in report.CategorySummaries)
    {
        reportData.AppendLine(
            $"{item.CategoryName}: Waste Cost = ₱{item.WasteCost:N2}");
    }

    reportData.AppendLine("\nWaste by Reason:");

    foreach (var item in report.ReasonSummaries)
    {
        reportData.AppendLine(
            $"{item.Reason}: Quantity Wasted = {item.QuantityWasted}, Waste Cost = ₱{item.WasteCost:N2}");
    }

    return reportData.ToString();
}

    }
}

