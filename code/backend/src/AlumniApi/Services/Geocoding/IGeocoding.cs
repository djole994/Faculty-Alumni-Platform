
    public interface IGeocodingService
    {
        Task<GeoCache> ResolveLocationAsync(string city, Country country);
    }
