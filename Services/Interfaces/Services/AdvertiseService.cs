using Common.Resources;
using Data;
using Data.Repositories;
using Entities.Base;
using Entities.Common.ViewModels;
using Entities.Models.User;
using EstateAgentApi.Services.Base;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.Services
{
    public class AdvertiseService : ServiceBase<AdvertiseService>, IAdvertiseService
    {
        private IRepository<User> _repo;
        private IRepository<UserAdvertises> _repoAd;
        private IRepository<UserRoles> _repoUR;
        private IRepository<Role> _repoR;
        ILogger<AdvertiseService> _logger;
        //private readonly IJwtService _jwtService;
        public AdvertiseService(ILogger<AdvertiseService> logger, IRepository<User> repository, ApplicationDbContext context,/* IJwtService jwtService,*/ IRepository<UserRoles> repoUR, IRepository<Role> repoR, IRepository<UserAdvertises> repoAd)  :base(logger)
        {
            _repo = repository;
            //_jwtService = jwtService;
            _repoUR = repoUR;
            _repoR = repoR;
            _repoAd = repoAd;
        }

        public async Task<ServiceResult> GetAllAdvertises(int pageId = 1, string advertiseText = "", string homeAddress = "")
        {
            try
            { 
                var result =  _repoAd.Entities.ToList();

                if (result is null || result.Count == 0)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                if (!string.IsNullOrEmpty(advertiseText))
                {
                    result = (List<UserAdvertises>)result.Where(r => r.AdvertiseText.Contains(advertiseText));
                  
                    if (result is null || result.Count == 0)
                        return NotFound(ErrorCodeEnum.NotFound, Resource.AdveriseNotFound, null);///
                }
                if (!string.IsNullOrEmpty(homeAddress))
                {
                    result = (List<UserAdvertises>)result.Where(r => r.HomeAddress.Contains(homeAddress));

                    if (result is null || result.Count == 0)
                        return NotFound(ErrorCodeEnum.NotFound, Resource.AdveriseNotFound, null);///
                }

                int take = 10;
                int skip = (pageId - 1) * take;    
                int pagecount = result.Select(c => new UserAdvertisesForHomePage()
                {
                    AdvertiseId = c.Id,
                    AdvertiseTitle = c.AdvertiseText,
                    AdvertiserName = c.AdvertiserName,                 
                    AdvertiserPhone = c.AdvertiserNumber,
                    Price = c.TotalPrice
          
                }).Count() / take;

                var query = result.Select(c => new UserAdvertisesForHomePage()
                {
                    AdvertiseId = c.Id,
                    AdvertiseTitle = c.AdvertiseText,
                    AdvertiserName = c.AdvertiserName,
                    AdvertiserPhone = c.AdvertiserNumber,
                    Price = c.TotalPrice

                }).Skip(skip).Take(take).ToList();


              var finalResult = Tuple.Create(query, pagecount);

                return Ok(finalResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
    }
}
