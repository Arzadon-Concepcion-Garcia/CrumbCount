
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrumbCount2.Models;
using CrumbCount2.Models.ViewModels;

namespace CrumbCount2.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
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

            return View(report);
        }
    }
}
