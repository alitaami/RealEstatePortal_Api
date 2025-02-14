using Common.Resources;
using Entities.Base;
using Entities.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAdvertiseService
    {
        public Task<ServiceResult> GetAllAdvertises(int pageId = 1, string advertiseText = "", string homeAddress = "", string orderBy = "date", string saleType = "sale", long startprice = 0, long endprice = 0, long startrentprice = 0, long endrentprice = 0);
        public Task<ServiceResult> SearchAdvertises_ElasticSearch( string advertiseText = "");
        public Task<ServiceResult> GetAdveriseForShow(int advertiseId);
        public Task<ServiceResult> GetAdvertiseImages(int advertiseId);
        public Task<ServiceResult> GetAdvertiseAvailableVisitDays(int advertiseId);
        public Task<ServiceResult> RequestForAdvertiseVisit(DateTimeOffset dayOfWeek, int advertiseId, int userId, string fullName);

        #region most used methods
        public Task<string> GetUserFullname(int userId);

        #endregion
    }
}
