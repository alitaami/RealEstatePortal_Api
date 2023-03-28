using Entities.Base;
using Entities.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        public Task<ServiceResult> GetUserInfo(int userId, CancellationToken cancellationToken);
        public Task<ServiceResult> UpdateUserInfo(int userId, UserPanelViewModel user, CancellationToken cancellationToken);
        public Task<ServiceResult> UpdateEstateAgentInfo(int userId, EstateAgentPanelViewModel user, CancellationToken cancellationToken);
        public Task<ServiceResult> CreateAdvertise(UserAdvertiseViewModel ua, int userId, CancellationToken cancellationToken);
        public Task<ServiceResult> GetAllAdvertisesOfUser(int pageId = 1, string advertiseText = "", string homeAddress = "", string orderBy = "date", string saleType = "sale", long startprice = 0, long endprice = 0, long startrentprice = 0, long endrentprice = 0, int userId = 0);
        public Task<ServiceResult> UpdateAdvertiseOfUser(int advertiseId, int userId, UserAdvertiseViewModel ua, CancellationToken cancellationToken);
        public Task<ServiceResult> DeleteAdvertiseOfUser(int advertiseId, int userId, CancellationToken cancellationToken);
        public Task<ServiceResult> CreateAdvertiseAvailableVisitDays(List<int> SelectedDays, int advertiseId, int userId);
        public Task<ServiceResult> UpdateAdvertiseAvailableVisitDays(List<int> SelectedDays, int advertiseId, int userId);
        public Task<ServiceResult> GetAdvertiseAvailableVisitDays(int advertiseId, int userId);
        public Task<ServiceResult> AdvertiserGetRequestsForVisit(int advertiseId, int userId);
        public Task<ServiceResult> AdvertiserConfirmRequestsForVisit(int reqId, int userId);
        public Task<ServiceResult> UserRequestsForVisit(int userId);
        public Task<ServiceResult> UserDeleteRequestsForVisit(int reqId, int userId);

        #region Mostly repeated codes
        public bool CheckUserHasThisAdvertise(int advertiseId, int userId);
        public Task<string> GetUserEmail(int userId);
        public Task<string> GetAdvertiseName(int advetiseId);
        #endregion

    }
}
