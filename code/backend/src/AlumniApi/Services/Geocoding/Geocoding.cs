using AlumniApi.Models.Caching;
using AlumniApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;
using AlumniApi.Helpers;
using AlumniApi.Models.AlProfile;
using System.Collections.Concurrent;

namespace AlumniApi.Services.GeocodingService
{
    public class Geocoding : IGeocodingService
    {
        private readonly AlumniContext _context;
        private readonly HttpClient _httpClient;

        // Lock po searchKey (sprečava duple upise za isti grad)
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        // GLOBALNI RATE LIMIT ZA NOMINATIM (1 zahtjev u sekundi)
        private static readonly SemaphoreSlim _nominatimRateLimit = new SemaphoreSlim(1, 1);

        public Geocoding(AlumniContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<GeoCache> ResolveLocationAsync(string inputCity, Country country)
        {
            // 1. GENERIŠEMO SEARCH KEY
            string searchKey = StringHelper.GenerateSearchKey(inputCity, country.Name);

            // 2. PRVI POKUŠAJ: CACHE (bez locka – brzi put)
            var cached = await _context.GeoCaches.AsNoTracking()
                .SingleOrDefaultAsync(x => x.SearchKey == searchKey);

            // Ako postoji zapis (čak i IsVerified = false), NE ZOVEMO API
            if (cached != null)
                return cached;

            // 3. LOCK PO SEARCH KEY-U (sprečava stampede)
            var sem = _locks.GetOrAdd(searchKey, _ => new SemaphoreSlim(1, 1));
            await sem.WaitAsync();

            try
            {
                // 4. DRUGA PROVJERA UNUTAR LOCKA
                cached = await _context.GeoCaches.AsNoTracking()
                    .SingleOrDefaultAsync(x => x.SearchKey == searchKey);

                if (cached != null)
                    return cached;

                // 5. KREIRAMO FALLBACK ZAPIS (KOORDINATE DRŽAVE)
                var newEntry = new GeoCache
                {
                    City = inputCity,
                    Country = country.Name,
                    SearchKey = searchKey,
                    IsVerified = false,
                    Latitude = country.DefaultLatitude,
                    Longitude = country.DefaultLongitude
                };

                // 6. POZIV NOMINATIM API-JA (RATE LIMIT: 1 REQ / SEK)
                try
                {
                    // ČEKAMO RED ZA NOMINATIM (GLOBALNO)
                    await _nominatimRateLimit.WaitAsync();

                    try
                    {
                        string url =
                            $"https://nominatim.openstreetmap.org/search?city={Uri.EscapeDataString(inputCity)}" +
                            $"&countrycodes={country.IsoCode.ToLower()}&format=json&limit=1&addressdetails=1";

                        var response = await _httpClient.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            using var doc = JsonDocument.Parse(json);

                            if (doc.RootElement.GetArrayLength() > 0)
                            {
                                var result = doc.RootElement[0];

                                // Našli smo tačan grad → prepisujemo fallback
                                newEntry.Latitude = double.Parse(
                                    result.GetProperty("lat").GetString()!,
                                    CultureInfo.InvariantCulture);

                                newEntry.Longitude = double.Parse(
                                    result.GetProperty("lon").GetString()!,
                                    CultureInfo.InvariantCulture);

                                newEntry.IsVerified = true;
                            }
                        }
                    }
                    finally
                    {
                        // OBEZBJEĐUJEMO 1 SEKUND RAZMAKA IZMEĐU API POZIVA
                        await Task.Delay(1000);
                        _nominatimRateLimit.Release();
                    }
                }
                catch
                {
                    // Ako API pukne, ostaje fallback (koordinate države)
                }

                // 7. UPIS U BAZU
                _context.GeoCaches.Add(newEntry);

                try
                {
                    await _context.SaveChangesAsync();
                    return newEntry;
                }
                catch (DbUpdateException ex) when (IsUniqueViolation(ex))
                {
                    // Ako je drugi thread/server već upisao isti SearchKey
                    return await _context.GeoCaches.AsNoTracking()
                        .SingleAsync(x => x.SearchKey == searchKey);
                }
            }
            finally
            {
                // 8. OTKLJUČAVANJE
                sem.Release();

                // Čišćenje lock-a da dictionary ne raste beskonačno
                if (sem.CurrentCount == 1)
                    _locks.TryRemove(searchKey, out _);
            }
        }

        private static bool IsUniqueViolation(DbUpdateException ex)
        {
            return ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
                   && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
        }
    }
}
