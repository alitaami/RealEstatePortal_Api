using Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.User.Advertises
{
    public class AdvertiseImages : BaseEntity
    {
        [Required]
        public int AdvertiseId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string FilePath { get; set; }
        [Required]
        public string FileName { get; set; }

        #region relations
        [ForeignKey(nameof(AdvertiseId))]
        public UserAdvertises UserAdvertises { get; set; }

        #endregion
    }
}
