using Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Common.Dtos
{
    public class AdvertiseVisitRequestsDto
    {
        [Required]
        public int AdvertiseId { get; set; }
 
        [Required]
        public string FullNameOfUser { get; set; }

        [Required]
        public DaysOfWeek DayOfWeek { get; set; }
        public bool IsConfirm { get; set; }
         
    }
}
