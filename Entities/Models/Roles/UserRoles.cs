using Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Roles
{
    public class UserRoles : BaseEntity
    {
        public UserRoles()
        {

        }

        public int UserId { get; set; }
        public int RoleId { get; set; }

        #region relations

        [ForeignKey(nameof(UserId))]
        public virtual User.User User { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }
        #endregion
    }
}
