
using System.Net.Http.Json;
using System.Text.Json;
using CrumbCount2.Models;

namespace CrumbCount2.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly HttpClient _http;

        public HolidayService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Holiday>> GetHolidaysAsync(
            int year,
            CancellationToken cancellationToken = default)
        {
            if (year < 1900 || year > 9999)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(year), "Invalid holiday year.");
            }

            var url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/PH";

            var holidays = await _http.GetFromJsonAsync<List<Holiday>>(
                url, cancellationToken);

            return holidays ?? new List<Holiday>();
        }
    }
}
