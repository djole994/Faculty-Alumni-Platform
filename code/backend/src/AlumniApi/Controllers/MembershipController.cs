
using AlumniApi.Models.AlProfile;
using AlumniApi.Services.GeocodingService;
using AlumniApi.DTOs;
using AlumniApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlumniApi.DTOs.MembershipDto;

[ApiController]
[Route("api/[controller]")]
public class MembershipController : ControllerBase
{
    private readonly AlumniContext _context;
    private readonly IGeocodingService _locationService;
    private readonly CaptchaService _captchaService;

    public MembershipController(AlumniContext context, IGeocodingService locationService, CaptchaService captchaService)
    {
        _context = context;
        _locationService = locationService;
        _captchaService = captchaService;
    }

    [HttpPost("apply")]
    [AllowAnonymous]
    public async Task<IActionResult> SubmitApplication([FromBody] MembershipApplicationDto dto, CancellationToken ct)
    {
        // 1. Provjera korisnika, emaila, dto anotacije
        if (!string.IsNullOrWhiteSpace(dto.Website))
        {
            await Task.Delay(Random.Shared.Next(150, 400), ct);
            return Ok(new { message = "✔️ Vaša pristupnica je primljena." });
        }
        if (User?.Identity?.IsAuthenticated == true && !User.IsInRole("Admin"))
            return Forbid();

        var email = dto.ContactEmail.Trim().ToLowerInvariant();

        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var captchaOk = await _captchaService.VerifyAsync(dto.CaptchaToken, ip);
        if (!captchaOk) return BadRequest("Captcha verifikacija nije prošla.");


        // 2. GEOLOCIRANJE
        // Servis će provjeriti keš, pa API, pa vratiti rezultat 
        var country = await _context.Countries.FindAsync(dto.CountryId);
        if (country == null) return BadRequest("Neispravna država.");
        var geoInfo = await _locationService.ResolveLocationAsync(dto.City, country);

        // 3. Kreiramo Profil
        var profile = new AlumniProfile
        {
            FullName = dto.FullName,
            DateOfBirth = dto.DateOfBirth,
            Address = dto.Address,
            City = dto.City,
            CountryId = country.Id,

            Latitude = geoInfo.Latitude,  
            Longitude = geoInfo.Longitude, 
            IsLocationVerified = geoInfo.IsVerified,
            GeoCacheId = geoInfo.Id,

            GraduationDate = dto.GraduationDate,
            StudyProgram = dto.StudyProgram,
            PhoneNumber = dto.PhoneNumber,
            ContactEmail = email,

            MembershipDate = DateTime.UtcNow,
            IsApproved = false, // Čeka admina

            AppUser = null,
            ProfilePicturePath = null 
        };

        try
        {
            var conn = _context.Database.GetDbConnection();
            _logger.LogWarning("DB: DataSource={DataSource} | Database={Database}",
                conn.DataSource, conn.Database);

            _context.AlumniProfiles.Add(profile);
            await _context.SaveChangesAsync();

            // 2) napravi email poruku (welcome + uputstvo)
            var registerUrl = "https://frontend-url/register"; 
            var msg = EmailTemplates.WelcomeToAlumni(dto.ContactEmail, dto.FullName, registerUrl);

            // 3) upiši u outbox (NE šaljem direktno)
            _context.EmailOutboxes.Add(new EmailOutbox
            {
                To = msg.To,
                Subject = msg.Subject,
                HtmlBody = msg.HtmlBody,
                TextBody = msg.TextBody,
                Status = EmailOutboxStatus.Pending,
                FailureCount = 0,
                NextAttemptAtUtc = DateTime.UtcNow,
                CorrelationKey = $"alumni-application:{profile.Id}"
            });

            // 4) save
            await _context.SaveChangesAsync(ct);

            return Ok(new { message = "✔️ Vaša pristupnica je primljena." });
        }


        catch (DbUpdateException ex) when (IsUniqueEmailViolation(ex))
        {

            _logger.LogInformation(ex, "Duplicate application suppressed. email={Email}", email);
            return Ok(new { message = "✔️ Vaša pristupnica je primljena." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply failed for email={Email}", email);
            throw;
        }
    }

    private static bool IsUniqueEmailViolation(DbUpdateException ex)
    {
            if (ex.InnerException is SqlException sql)
                return sql.Number is 2601 or 2627;

            return false;
    }




    [HttpGet("map/public")]
    public async Task<ActionResult<List<MapLocationPublicDto>>> GetPublicMap(
        [FromQuery] bool verifiedOnly = true,
        [FromQuery] int precision = 2 // 2 ≈ ~1km, 3 ≈ ~100m
    )
    {
        precision = Math.Clamp(precision, 1, 6);

        // Uzimamo samo koordinatu + GeoCacheId (za grupisanje), bez PII
        var rows = await _context.AlumniProfiles
            .AsNoTracking()
            .Where(p => p.IsApproved)
            .Where(p => p.Latitude != null && p.Longitude != null)
            .Where(p => !verifiedOnly || p.IsLocationVerified)
            .Select(p => new
            {
                Lat = p.Latitude!.Value,
                Lng = p.Longitude!.Value,
                p.GeoCacheId
            })
            .ToListAsync();

        static double R(double v, int dec) => Math.Round(v, dec);

        string KeyFor(double lat, double lng, int? geoCacheId)
        {
            // Ako ima GeoCacheId grupiši po njemu
            if (geoCacheId != null) return $"g:{geoCacheId}";

            // Ako nema, grupiši po zaokruženim koordinatama (privatnost + grupisanje)
            return $"c:{R(lat, precision)}|{R(lng, precision)}";
        }

        var groups = rows
            .GroupBy(r => KeyFor(r.Lat, r.Lng, r.GeoCacheId))
            .Select(g =>
            {
                var first = g.First();
                return new MapLocationPublicDto(
                    Lat: R(first.Lat, precision),
                    Lng: R(first.Lng, precision),
                    Count: g.Count()
                );
            })
            .ToList();

        return Ok(groups);
    }

}
