using System.ComponentModel.DataAnnotations;
using AlumniApi.Resources;

namespace AlumniApi.DTOs
{
    public class MembershipApplicationDto
    {
        [Display(Name = "FullName", ResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength_Max")]
        public string FullName { get; set; } = string.Empty;


        [Display(Name = "Email", ResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "EmailInvalid")]
        [StringLength(254, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength_Max")]
        public string ContactEmail { get; set; } = string.Empty;


        [Display(Name = "DateOfBirth", ResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        public DateTime DateOfBirth { get; set; }


        [Display(Name = "Address", ResourceType = typeof(ValidationMessages))]
        [StringLength(200, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength_Max")]
        public string? Address { get; set; }


        [Display(Name = "City", ResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength_Max")]
        public string City { get; set; } = string.Empty;


        [Display(Name = "Country", ResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        public int CountryId { get; set; }


        [Display(Name = "PhoneNumber", ResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Phone(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "PhoneInvalid")]
        [StringLength(30, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength_Max")]
        public string PhoneNumber { get; set; } = string.Empty;


        [Display(Name = "GraduationDate", ResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        public DateTime GraduationDate { get; set; }


        [Display(Name = "StudyProgram", ResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength_Max")]
        public string StudyProgram { get; set; } = string.Empty;


        [Display(Name = "Captcha", ResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        public string CaptchaToken { get; set; } = "";


        [Display(Name = "AgreeToPrivacy", ResourceType = typeof(ValidationMessages))]
        [Range(typeof(bool), "true", "true",
            ErrorMessageResourceType = typeof(ValidationMessages),
            ErrorMessageResourceName = "MustAgreeToPrivacy")]
        public bool AgreeToPrivacy { get; set; }
    }
}
