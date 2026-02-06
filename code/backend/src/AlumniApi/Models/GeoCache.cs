using System.ComponentModel.DataAnnotations;

namespace AlumniApi.Models.Caching
{
    public class GeoCache
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string City { get; set; } = "";

        [MaxLength(100)]
        public string Country { get; set; } = "";

        public string SearchKey { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [MaxLength(200)]
        public string? DisplayName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsVerified { get; set; }
    }

}
