using Entities.Common;
using Entities.Models.Advertises;
using Entities.Models.Roles;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models.User
{
    public class User : BaseEntity
    {
        public User()
        {
            IsActive = false;
            SecurityStamp = Guid.NewGuid();
            ActivationGuid = Guid.NewGuid();
         }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "That should be in email format!")]
        [StringLength(100)]
        public string Email { get; set; }
        [Required]
        [StringLength(500)]
        public string PasswordHash { get; set; }
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        [Required]
        public int Age { get; set; }
        public bool IsActive { get; set; }
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
        public Guid SecurityStamp { get; set; }
        public Guid ActivationGuid { get; set; }
   
        #region Relations
        public virtual List<UserRoles> UserRoles { get; set; }

        public virtual List<UserAdvertises> UserAdvertises { get; set; }
        public virtual List<UserForgetPassword> UserForgetPassword { get; set; }


        #endregion
    }

}
