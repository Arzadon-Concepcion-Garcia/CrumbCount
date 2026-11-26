
using CrumbCount2.Models;

namespace CrumbCount2.Services
{
    public interface IHolidayService
    {
        Task<List<Holiday>> GetHolidaysAsync(
            int year,
            CancellationToken cancellationToken = default);
    }
}
