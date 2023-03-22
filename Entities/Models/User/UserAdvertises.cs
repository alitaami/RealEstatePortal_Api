using Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.User
{
    public class UserAdvertises : BaseEntity
    {

        public int UserId { get; set; }
        [Required]
        [StringLength(100)]
        public string AdvertiseText { get; set; }
        [Required]
        [StringLength(50)]
        public string AdvertiserName { get; set; }

        [Required]
        [StringLength(11)]
        public string AdvertiserNumber { get; set; }
        [Required]
        [StringLength(200)]
        public string HomeAddress { get; set; }
        [Required]
        public int RoomCount { get; set; }
        [Required]
        public long Meterage { get; set; }

        [Required]
        public long PricePerMeter { get; set; }
        [Required]
        public long TotalPrice { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public bool HasElevator { get; set; }
        public bool HasBalcony { get; set; }
        public bool HasWarehouse { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        #region relations

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }


        #endregion



    }
}
