public class AlumniProfile
{
    public int Id { get; set; }

    public string FullName { get; set; } = "";
    public string ContactEmail { get; set; } = "";

    public string City { get; set; } = "";
    public int CountryId { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsLocationVerified { get; set; }
    public int? GeoCacheId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsApproved { get; set; }
}
