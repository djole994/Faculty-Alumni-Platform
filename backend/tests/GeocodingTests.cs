using Xunit;
using Moq;
using Moq.Protected; 
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using AlumniApi.Services.GeocodingService;
using AlumniApi.Models;
using AlumniApi.Models.Caching;
using AlumniApi.Helpers;
using AlumniApi.Models.AlProfile; 

public class GeocodingTests
{
    private readonly AlumniContext _fakeDb;

    public GeocodingTests()
    {
        var options = new DbContextOptionsBuilder<AlumniContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _fakeDb = new AlumniContext(options);
    }

    // POMOĆNA METODA:lažni HttpClient
    private HttpClient CreateMockHttpClient(string responseJson, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            });

        return new HttpClient(handlerMock.Object);
    }

    [Fact]
    public async Task ResolveLocation_ReturnsFromCache_IfExists()
    {
        // --- ARRANGE ---
        var country = new Country { Name = "BiH", IsoCode = "ba", DefaultLatitude = 44, DefaultLongitude = 18 };
        var city = "Foca";
        var searchKey = StringHelper.GenerateSearchKey(city, country.Name);

        _fakeDb.GeoCaches.Add(new GeoCache
        {
            SearchKey = searchKey,
            City = city,
            Country = country.Name,
            Latitude = 50.0, // fake coords
            Longitude = 50.0,
            IsVerified = true
        });
        await _fakeDb.SaveChangesAsync();

        // Pravimo servis sa dummy klijentom (neće se ni pozvati1)
        var service = new LocationService(_fakeDb, new HttpClient());

        // --- ACT ---
        var result = await service.ResolveLocationAsync(city, country);

        // --- ASSERT ---
        Assert.Equal(50.0, result.Latitude); // Mora vratiti ono iz baze
        Assert.True(result.IsVerified);
    }

    [Fact]
    public async Task ResolveLocation_CallsApi_AndSavesToDb_WhenCacheEmpty()
    {
        // --- ARRANGE ---
        var country = new Country { Name = "BiH", IsoCode = "ba", DefaultLatitude = 44, DefaultLongitude = 18 };
        var city = "Banja Luka";

        // fake json
        var jsonResponse = "[{\"lat\": \"44.538\", \"lon\": \"18.667\", \"display_name\": \"Banja Luka, BiH\"}]";

        var mockHttp = CreateMockHttpClient(jsonResponse);
        var service = new LocationService(_fakeDb, mockHttp);

        // --- ACT ---
        var result = await service.ResolveLocationAsync(city, country);

        // --- ASSERT ---
        // 1. Provjera rezultata
        Assert.Equal(44.538, result.Latitude);
        Assert.Equal(18.667, result.Longitude);
        Assert.True(result.IsVerified);

        // 2. Provjera da li je snimljeno u bazu
        var dbEntry = await _fakeDb.GeoCaches.FirstOrDefaultAsync(x => x.City == "Banja Luka");
        Assert.NotNull(dbEntry);
        Assert.Equal(44.538, dbEntry.Latitude);
    }

    [Fact]
    public async Task ResolveLocation_UsesFallback_WhenApiFails()
    {
        // --- ARRANGE ---
        var country = new Country { Name = "BiH", IsoCode = "ba", DefaultLatitude = 44.0, DefaultLongitude = 18.0 };

        // API vraća prazan niz [] (nije našao grad) ili grešku
        var mockHttp = CreateMockHttpClient("[]");
        var service = new LocationService(_fakeDb, mockHttp);

        // --- ACT ---
        var result = await service.ResolveLocationAsync("NepostojeciGrad", country);

        // --- ASSERT ---
        // Mora vratiti default koordinate države (Fallback)
        Assert.Equal(44.0, result.Latitude);
        Assert.Equal(18.0, result.Longitude);
        Assert.False(result.IsVerified); // Nije verifikovano

        // I to mora biti upisano u bazu da ne bi opet zvali API
        Assert.NotNull(await _fakeDb.GeoCaches.FirstOrDefaultAsync(x => x.City == "NepostojeciGrad"));
    }
}
