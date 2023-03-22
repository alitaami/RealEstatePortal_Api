using Common.Resources;
using System.ComponentModel.DataAnnotations;

namespace Entities.Common.ViewModels
{
    public class UserViewModel : IValidatableObject
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(500)]
        [PasswordQuality(8, 4, ErrorMessageResourceName = "PasswordQuality", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "That should be in email format!")]
        [StringLength(100)]
        public string Email { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        [StringLength(11)]
        public string PhoneNumber { get; set; }
        public bool IsEstateConsultant { get; set; }
        [StringLength(200)]
        public string? EstateAddress { get; set; }

        [StringLength(11)]
        public string? EstatePhoneNumber { get; set; }
        [StringLength(10)]
        public string? EstateCode { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsEstateConsultant)
            {
                if(EstateAddress is null && EstatePhoneNumber is null && EstateCode is null )
                yield return new ValidationResult(Resource.EstateInfo);
            }

        }
    }
}
