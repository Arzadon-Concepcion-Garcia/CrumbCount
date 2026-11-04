
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrumbCount2.Models;

namespace CrumbCount2.Controllers;

public class DispositionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public DispositionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Display all dispositions
    public async Task<IActionResult> Index()
    {
        var dispositions = _context.Dispositions
            .Include(d => d.WasteRecording)
            .ThenInclude(w => w!.Product);

        return View(await dispositions.ToListAsync());
    }

    // Display the create page
    public IActionResult Create()
    {
        ViewData["WasteRecordingId"] = new SelectList(
            _context.WasteRecordings
                .Include(w => w.Product)
                .Select(w => new
                {
                    w.WasteRecordingId,
                    ProductName = w.Product!.ProductName
                }),
            "WasteRecordingId",
            "ProductName");

        return View();
    }

    // Save a new disposition
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Disposition disposition)
    {
        if (ModelState.IsValid)
        {
            _context.Add(disposition);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        ViewData["WasteRecordingId"] = new SelectList(
            _context.WasteRecordings,
            "WasteRecordingId",
            "WasteRecordingId",
            disposition.WasteRecordingId);

        return View(disposition);
    }
}
