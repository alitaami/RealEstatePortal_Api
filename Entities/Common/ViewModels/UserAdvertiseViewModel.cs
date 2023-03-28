using Entities.Common.Enums;
using Entities.Models.User;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Common.ViewModels
{
    public class UserAdvertiseViewModel
    {
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

        public bool ForSale { get; set; }

        public long Meterage { get; set; }

        public long? PricePerMeter { get; set; }

        public long? DespositPrice { get; set; }
        public long? RentPrice { get; set; }

        [Required]
        public BuildingTypeEnum BuildingType { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
        public bool HasElevator { get; set; }
        public bool HasBalcony { get; set; }
        public bool HasWarehouse { get; set; }
        public bool HasGarage { get; set; }


    }

    public class UserAdvertiseViewModelForAdmin
    {
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

        public bool ForSale { get; set; }

        public long Meterage { get; set; }

        public long? PricePerMeter { get; set; }

        public long? DespositPrice { get; set; }
        public long? RentPrice { get; set; }

        [Required]
        public BuildingTypeEnum BuildingType { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
        public bool HasElevator { get; set; }
        public bool HasBalcony { get; set; }
        public bool HasWarehouse { get; set; }
        public bool HasGarage { get; set; }
        public bool IsDelete { get; set; }
        public bool IsConfirm { get; set; }

    }

    public class UserAdvertisesForHomePage
    {
        public int AdvertiseId { get; set; }
        public string AdvertiseTitle { get; set; }
        public string AdvertiserName { get; set; }
        public string AdvertiserPhone { get; set; }
        public bool ForSale { get; set; }
        public long? Price { get; set; }
        public long? DespositPrice { get; set; }
        public long? RentPrice { get; set; }


    }

   

}
