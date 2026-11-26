
using System.Diagnostics;
using CrumbCount2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrumbCount2.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        ViewBag.ProductCount = _context.Products.Count();
        ViewBag.CategoryCount = _context.Categories.Count();
        ViewBag.WasteCount = _context.WasteRecordings.Count();
        ViewBag.DispositionCount = _context.Dispositions.Count();

        var recentWaste = _context.WasteRecordings
            .Include(w => w.Product)
            .OrderByDescending(w => w.DateRecorded)
            .Take(5)
            .ToList();

        return View(recentWaste);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
