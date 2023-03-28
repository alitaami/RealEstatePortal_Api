using Common.Resources;
using Entities.Models.User;
using System.ComponentModel.DataAnnotations;

namespace Entities.Common.ViewModels
{
    public class UserViewModel
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(500)]
        [PasswordQuality(8, 4, ErrorMessageResourceName = "PasswordQuality", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; }

        [Compare(nameof(Password))]
        public string RePassword { get; set; }

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

        public DateTimeOffset? LastLoginDate { get; set; }

    }
    public class EstateUserViewModel
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(500)]
        [PasswordQuality(8, 4, ErrorMessageResourceName = "PasswordQuality", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; }

        [Compare(nameof(Password))]
        public string RePassword { get; set; }

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

        [Required]
        [StringLength(200)]
        public string EstateAddress { get; set; }
        [Required]
        [StringLength(11)]
        public string EstatePhoneNumber { get; set; }
        [Required]
        [StringLength(10)]
        public string EstateCode { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }

    }
    public class UserPanelViewModel
    {
        public string? OldPassword { get; set; }

        public string? NewPassword { get; set; }

        [Compare(nameof(NewPassword))]
        public string? RePassword { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        public int Age { get; set; }

    }

    public class EstateAgentPanelViewModel
    {

        public string? OldPassword { get; set; }

        public string? NewPassword { get; set; }

        [Compare(nameof(NewPassword))]
        public string? RePassword { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        [StringLength(200)]
        public string EstateAddress { get; set; }
        [Required]
        [StringLength(11)]
        public string EstatePhoneNumber { get; set; }
        [Required]
        [StringLength(10)]
        public string EstateCode { get; set; }
    }

    public class  UserViewModelFromAdmin
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(500)]
        [PasswordQuality(8, 4, ErrorMessageResourceName = "PasswordQuality", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; }

        [Compare(nameof(Password))]
        public string RePassword { get; set; }

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
        [Required]
        [StringLength(200)]
        public string EstateAddress { get; set; }
        [Required]
        [StringLength(11)]
        public string EstatePhoneNumber { get; set; }
        [Required]
        [StringLength(10)]
        public string EstateCode { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }

        public bool IsActive { get; set; }

    }

    public class EditUserViewModelFromAdmin
    {
  
        [Required]
        [StringLength(500)]
        [PasswordQuality(8, 4, ErrorMessageResourceName = "PasswordQuality", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; }

        [Compare(nameof(Password))]
        public string RePassword { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "That should be in email format!")]
        [StringLength(100)]
        public string Email { get; set; }
        [Required]
        public int Age { get; set; }

        public bool IsEstateConsultant { get; set; }
        [Required]
        [StringLength(200)]
        public string EstateAddress { get; set; }
        [Required]
        [StringLength(11)]
        public string EstatePhoneNumber { get; set; }
 
        public DateTimeOffset? LastLoginDate { get; set; }

        public bool IsActive { get; set; }

    }

}
