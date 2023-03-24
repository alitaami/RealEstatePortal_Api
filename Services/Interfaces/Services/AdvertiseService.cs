using Common.Resources;
using Data;
using Data.Repositories;
using Entities.Base;
using Entities.Common.Dtos;
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
        public AdvertiseService(ILogger<AdvertiseService> logger, IRepository<User> repository, ApplicationDbContext context,/* IJwtService jwtService,*/ IRepository<UserRoles> repoUR, IRepository<Role> repoR, IRepository<UserAdvertises> repoAd) : base(logger)
        {
            _repo = repository;
            //_jwtService = jwtService;
            _repoUR = repoUR;
            _repoR = repoR;
            _repoAd = repoAd;
        }
        #region EatateAgent
        public async Task<ServiceResult> CreateAdvertise(UserAdvertiseViewModel ua, int userId, CancellationToken cancellationToken)
        {
            try
            {
                ValidateModel(ua);

                UserAdvertises u = new UserAdvertises();

                var ud = await _repoAd.TableNoTracking
                       .FirstOrDefaultAsync(u => u.AdvertiseText == ua.AdvertiseText);

                if (ud != null)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.AdExists, null);///

                u.UserId = userId;
                u.AdvertiseText = ua.AdvertiseText;
                u.AdvertiserName = ua.AdvertiserName;
                u.AdvertiserNumber = ua.AdvertiserNumber;
                u.HomeAddress = ua.HomeAddress;
                u.Meterage = ua.Meterage;
                u.PricePerMeter = ua.PricePerMeter;
                u.TotalPrice = ua.Meterage * ua.PricePerMeter;
                u.RoomCount = ua.RoomCount;
                u.HasBalcony = ua.HasBalcony;
                u.HasElevator = ua.HasElevator;
                u.HasWarehouse = ua.HasWarehouse;
                u.Description = ua.Description;
                u.CreatedDate = DateTimeOffset.Now;

                await _repoAd.AddAsync(u, cancellationToken);

                var result = new UserAdvertiseDto
                {
                    AdvertiserName = ua.AdvertiserName,
                    AdvertiserNumber = ua.AdvertiserNumber,
                    AdvertiseText = ua.AdvertiseText,
                    HomeAddress = ua.HomeAddress,
                    Meterage = ua.Meterage,
                    PricePerMeter = ua.PricePerMeter,
                    TotalPrice = ua.Meterage * ua.PricePerMeter,
                    RoomCount = ua.RoomCount,
                    HasBalcony = ua.HasBalcony,
                    HasElevator = ua.HasElevator,
                    HasWarehouse = ua.HasWarehouse,
                    Description = ua.Description,
                    CreatedDate = u.CreatedDate
                };

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }

        public async Task<ServiceResult> GetAllAdvertisesOfAgent(int pageId = 1, string advertiseText = "", string homeAddress = "", int userId = 0)
        {
            try
            {
                if (userId == 0)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.GeneralErrorTryAgain, null);///

                IQueryable<UserAdvertises> result = _repoAd.Entities.Where(u => u.UserId == userId);


                if (result is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                if (!string.IsNullOrEmpty(advertiseText))
                {
                    result = result.Where(r => r.AdvertiseText.Contains(advertiseText));

                }
                if (!string.IsNullOrEmpty(homeAddress))
                {
                    result = result.Where(r => r.HomeAddress.Contains(homeAddress));

                }

                int take = 10;
                int skip = (pageId - 1) * take;
                int pagecount = result.Select(ua => new UserAdvertiseDto()
                {
                    AdvertiserName = ua.AdvertiserName,
                    AdvertiserNumber = ua.AdvertiserNumber,
                    AdvertiseText = ua.AdvertiseText,
                    HomeAddress = ua.HomeAddress,
                    Meterage = ua.Meterage,
                    PricePerMeter = ua.PricePerMeter,
                    TotalPrice = ua.Meterage * ua.PricePerMeter,
                    RoomCount = ua.RoomCount,
                    HasBalcony = ua.HasBalcony,
                    HasElevator = ua.HasElevator,
                    HasWarehouse = ua.HasWarehouse,
                    Description = ua.Description,
                    CreatedDate = ua.CreatedDate

                }).Count() / take;

                var query = result.Select(ua => new UserAdvertiseDto()
                {
                    AdvertiserName = ua.AdvertiserName,
                    AdvertiserNumber = ua.AdvertiserNumber,
                    AdvertiseText = ua.AdvertiseText,
                    HomeAddress = ua.HomeAddress,
                    Meterage = ua.Meterage,
                    PricePerMeter = ua.PricePerMeter,
                    TotalPrice = ua.TotalPrice,
                    RoomCount = ua.RoomCount,
                    HasBalcony = ua.HasBalcony,
                    HasElevator = ua.HasElevator,
                    HasWarehouse = ua.HasWarehouse,
                    Description = ua.Description,
                    CreatedDate = ua.CreatedDate

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

        public async Task<ServiceResult> UpdateAdvertiseOfAgent(int advertiseId, UserAdvertiseViewModel ua, CancellationToken cancellationToken)
        {
            try
            {
                var uAd = await _repoAd.GetByIdAsync(cancellationToken, advertiseId);

                if (uAd is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                uAd.AdvertiserName = ua.AdvertiserName;
                uAd.AdvertiserNumber = ua.AdvertiserNumber;
                uAd.AdvertiseText = ua.AdvertiseText;
                uAd.HomeAddress = ua.HomeAddress;
                uAd.Meterage = ua.Meterage;
                uAd.PricePerMeter = ua.PricePerMeter;
                uAd.TotalPrice = ua.Meterage * ua.PricePerMeter;
                uAd.RoomCount = ua.RoomCount;
                uAd.HasBalcony = ua.HasBalcony;
                uAd.HasElevator = ua.HasElevator;
                uAd.HasWarehouse = ua.HasWarehouse;
                uAd.Description = ua.Description;
                uAd.CreatedDate = DateTimeOffset.Now;

                _repoAd.Update(uAd);

                var result = new UserAdvertiseDto
                {
                    AdvertiserName = uAd.AdvertiserName,
                    AdvertiserNumber = uAd.AdvertiserNumber,
                    AdvertiseText = uAd.AdvertiseText,
                    HomeAddress = uAd.HomeAddress,
                    Meterage = uAd.Meterage,
                    PricePerMeter = uAd.PricePerMeter,
                    TotalPrice = uAd.Meterage * uAd.PricePerMeter,
                    RoomCount = uAd.RoomCount,
                    HasBalcony = uAd.HasBalcony,
                    HasElevator = uAd.HasElevator,
                    HasWarehouse = uAd.HasWarehouse,
                    Description = uAd.Description,
                    CreatedDate = uAd.CreatedDate
                };

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
        #endregion

        #region public
        public async Task<ServiceResult> GetAdveriseForShow(int advertiseId)
        {
            try
            {
                var ua = await _repoAd.TableNoTracking
                .FirstOrDefaultAsync(u => u.Id == advertiseId);

                if (ua == null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.AdveriseNotFound, null);///

                var result = new UserAdvertiseDto
                {

                    AdvertiserName = ua.AdvertiserName,
                    AdvertiserNumber = ua.AdvertiserNumber,
                    AdvertiseText = ua.AdvertiseText,
                    HomeAddress = ua.HomeAddress,
                    Meterage = ua.Meterage,
                    PricePerMeter = ua.PricePerMeter,
                    TotalPrice = ua.TotalPrice,
                    RoomCount = ua.RoomCount,
                    HasBalcony = ua.HasBalcony,
                    HasElevator = ua.HasElevator,
                    HasWarehouse = ua.HasWarehouse,
                    Description = ua.Description,

                };

                return Ok(result);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }

        public async Task<ServiceResult> GetAllAdvertises(int pageId = 1, string advertiseText = "", string homeAddress = "")
        {
            try
            {
                IQueryable<UserAdvertises> result = _repoAd.Entities;

                if (result is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                if (!string.IsNullOrEmpty(advertiseText))
                {
                    result = result.Where(r => r.AdvertiseText.Contains(advertiseText));

                }
                if (!string.IsNullOrEmpty(homeAddress))
                {
                    result = result.Where(r => r.HomeAddress.Contains(homeAddress));

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


        #endregion
    }
}
