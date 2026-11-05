
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrumbCount2.Models;

namespace CrumbCount2.Controllers
{
    public class WasteRecordingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WasteRecordingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WasteRecordings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.WasteRecordings
                .Include(w => w.Product);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: WasteRecordings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wasteRecording = await _context.WasteRecordings
                .Include(w => w.Product)
                .FirstOrDefaultAsync(m => m.WasteRecordingId == id);

            if (wasteRecording == null)
            {
                return NotFound();
            }

            return View(wasteRecording);
        }

        // GET: WasteRecordings/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(
                _context.Products,
                "ProductId",
                "ProductName"
            );

            return View();
        }

        // POST: WasteRecordings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("WasteRecordingId,ProductId,QuantityWasted,DateRecorded,ReasonForWaste,Remarks")]
            WasteRecording wasteRecording)
        {
            // Check if the selected product exists.
            bool productExists = await _context.Products
                .AnyAsync(p => p.ProductId == wasteRecording.ProductId);

            if (!productExists)
            {
                ModelState.AddModelError(
                    nameof(WasteRecording.ProductId),
                    "Please select a product that exists."
                );
            }

            // Save only when all validation rules pass.
            if (ModelState.IsValid)
            {
                _context.WasteRecordings.Add(wasteRecording);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Return the form with validation messages.
            ViewData["ProductId"] = new SelectList(
                _context.Products,
                "ProductId",
                "ProductName",
                wasteRecording.ProductId
            );

            return View(wasteRecording);
        }

        // GET: WasteRecordings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wasteRecording = await _context.WasteRecordings
                .FindAsync(id);

            if (wasteRecording == null)
            {
                return NotFound();
            }

            ViewData["ProductId"] = new SelectList(
                _context.Products,
                "ProductId",
                "ProductName",
                wasteRecording.ProductId
            );

            return View(wasteRecording);
        }

        // POST: WasteRecordings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("WasteRecordingId,ProductId,QuantityWasted,DateRecorded,ReasonForWaste,Remarks")]
            WasteRecording wasteRecording)
        {
            if (id != wasteRecording.WasteRecordingId)
            {
                return NotFound();
            }

            // Check if the selected product exists.
            bool productExists = await _context.Products
                .AnyAsync(p => p.ProductId == wasteRecording.ProductId);

            if (!productExists)
            {
                ModelState.AddModelError(
                    nameof(WasteRecording.ProductId),
                    "Please select a product that exists."
                );
            }

            // Update only when all validation rules pass.
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wasteRecording);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WasteRecordingExists(wasteRecording.WasteRecordingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // Return the form with validation messages.
            ViewData["ProductId"] = new SelectList(
                _context.Products,
                "ProductId",
                "ProductName",
                wasteRecording.ProductId
            );

            return View(wasteRecording);
        }

        // GET: WasteRecordings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wasteRecording = await _context.WasteRecordings
                .Include(w => w.Product)
                .FirstOrDefaultAsync(m => m.WasteRecordingId == id);

            if (wasteRecording == null)
            {
                return NotFound();
            }

            return View(wasteRecording);
        }

        // POST: WasteRecordings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wasteRecording = await _context.WasteRecordings
                .FindAsync(id);

            if (wasteRecording != null)
            {
                _context.WasteRecordings.Remove(wasteRecording);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Check if a waste record exists.
        private bool WasteRecordingExists(int id)
        {
            return _context.WasteRecordings
                .Any(e => e.WasteRecordingId == id);
        }
    }
}
