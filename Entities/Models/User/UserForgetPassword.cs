using Entities.Common;
using Entities.Models.User.Roles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.User
{
    public class UserForgetPassword : BaseEntity
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string RecoveryKey { get; set; }
        [Required]
        public DateTimeOffset CreatedDate { get; set; }
        [Required]
        public DateTimeOffset ExpireDate { get; set; }

        #region relations

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
 
        #endregion
    }
}
