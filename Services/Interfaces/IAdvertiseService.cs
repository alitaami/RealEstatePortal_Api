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
        public Task<ServiceResult> GetAllAdvertises(int pageId = 1, string advertiseText = "", string homeAddress = "");
        public Task<ServiceResult> GetAdveriseForShow(int advertiseId);
        public Task<ServiceResult> CreateAdvertise(UserAdvertiseViewModel ua, int userId, CancellationToken cancellationToken);
        public Task<ServiceResult> GetAllAdvertisesOfAgent(int pageId = 1, string advertiseText = "", string homeAddress = "", int userId=0);
        public Task<ServiceResult> UpdateAdvertiseOfAgent(int advertiseId, int userId, UserAdvertiseViewModel ua, CancellationToken cancellationToken);
        public Task<ServiceResult> DeleteAdvertiseOfAgent(int advertiseId, int userId,  CancellationToken cancellationToken);
        public Task<ServiceResult> GetEstateAgentInfo( int userId, CancellationToken cancellationToken);
        public Task<ServiceResult> UpdateEstateAgentInfo( int userId,EstateAgentPanelViewModel user, CancellationToken cancellationToken);

    }
}
