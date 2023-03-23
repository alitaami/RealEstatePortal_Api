using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Common.Dtos
{
    public class UserAdvertiseDto
    {
        public string AdvertiseText { get; set; }
        public string AdvertiserName { get; set; }
        public string AdvertiserNumber { get; set; }
        public string HomeAddress { get; set; }
        public int RoomCount { get; set; }
        public long Meterage { get; set; }
        public long PricePerMeter { get; set; }
        public long TotalPrice { get; set; }
        public bool HasElevator { get; set; }
        public bool HasBalcony { get; set; }
        public bool HasWarehouse { get; set; }
        public string Description { get; set; }


    }
}
