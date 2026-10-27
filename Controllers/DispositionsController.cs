
using CrumbCount2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CrumbCount2.Controllers
{
    public class DispositionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DispositionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dispositions = _context.Dispositions
                .Include(d => d.WasteRecording);

            return View(await dispositions.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            await LoadWasteRecordings();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Disposition disposition)
        {
            if (ModelState.IsValid)
            {
                _context.Dispositions.Add(disposition);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await LoadWasteRecordings(disposition.WasteRecordingId);
            return View(disposition);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var disposition = await _context.Dispositions.FindAsync(id);

            if (disposition == null)
                return NotFound();

            await LoadWasteRecordings(disposition.WasteRecordingId);
            return View(disposition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Disposition disposition)
        {
            if (id != disposition.DispositionId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(disposition);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await LoadWasteRecordings(disposition.WasteRecordingId);
            return View(disposition);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var disposition = await _context.Dispositions
                .Include(d => d.WasteRecording)
                .FirstOrDefaultAsync(d => d.DispositionId == id);

            if (disposition == null)
                return NotFound();

            return View(disposition);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var disposition = await _context.Dispositions
                .FindAsync(id);

            if (disposition != null)
            {
                _context.Dispositions.Remove(disposition);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadWasteRecordings(int? selectedId = null)
        {
            var recordings = await _context.WasteRecordings
                .OrderByDescending(w => w.DateRecorded)
                .ToListAsync();

            ViewBag.WasteRecordingId = new SelectList(
                recordings,
                "WasteRecordingId",
                "WasteRecordingId",
                selectedId);
        }
    }
}
