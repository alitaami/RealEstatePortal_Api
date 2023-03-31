using Entities.Common;
using Entities.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Advertises
{
    public class AdvertiseAvailableVisitDays : BaseEntity
    {
        [Required]
        public int AdvertiseId { get; set; }
        [Required]
        public DateTimeOffset AvailableVisitDay { get; set; }

        #region relations
        [ForeignKey(nameof(AdvertiseId))]
        public UserAdvertises UserAdvertises { get; set; }
        #endregion
    }
}
