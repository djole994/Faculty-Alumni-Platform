
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
    public async Task<IActionResult> SubmitApplication([FromBody] MembershipApplicationDto dto)
    {
        // 1. Provera emaila, dto anotacije
        var email = dto.ContactEmail.Trim().ToLowerInvariant();
        if (await _context.AlumniProfiles.AnyAsync(p => p.ContactEmail == email))
        {
            return BadRequest("Pristupnica sa ovim emailom već postoji.");
        }

        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var captchaOk = await _captchaService.VerifyAsync(dto.CaptchaToken, ip);
        if (!captchaOk) return BadRequest("Captcha verifikacija nije prošla.");

        // 2. GEOLOCIRANJE 
        // Servis će proveriti keš, pa API, pa vratiti rezultat 
        var country = await _context.Countries.FindAsync(dto.CountryId);
        if (country == null) return BadRequest("Neispravna država.");
        var geoInfo = await _locationService.ResolveLocationAsync(dto.City, country);

        // 3. 
        var profile = new AlumniProfile
        {
            FullName = dto.FullName,
            ContactEmail = email,

            City = dto.City,
            CountryId = country.Id,

            Latitude = geoInfo.Latitude,  
            Longitude = geoInfo.Longitude, 
            IsLocationVerified = geoInfo.IsVerified,
            GeoCacheId = geoInfo.Id,

            MembershipDate = DateTime.UtcNow,
            IsApproved = false // Čeka admina
        };

        _context.AlumniProfiles.Add(profile);
        await _context.SaveChangesAsync();

        //generisati neki smtp za obavijest adminu i alumnisti o novoj pristupnici, naknadno dodajem

        // 4. Obavještenje (ako lokacija nije nađena)
        string warningMsg = "";
        if (!geoInfo.IsVerified)
        {
            warningMsg = "Napomena: Nismo uspjeli automatski da lociramo vaš grad na mapi, administrator će to ručno pregledati.";
        }

        return Ok(new { message = "Vaša pristupnica je primljena." + warningMsg });
    }

}
