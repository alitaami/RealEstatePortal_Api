using Common.Resources;
using Common.Utilities;
using Data;
using Data.Repositories;
using Entities.Base;
using Entities.Common.Dtos;
using Entities.Common.Enums;
using Entities.Common.ViewModels;
using Entities.Models.User;
using Entities.Models.User.Advertises;
using Entities.Models.User.Roles;
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
using static Entities.Common.Dtos.UserAdvertiseDto;

namespace Services.Interfaces.Services
{
    public class AdvertiseService : ServiceBase<AdvertiseService>, IAdvertiseService
    {
        private IRepository<User> _repo;
        private IRepository<UserAdvertises> _repoAd;
        private IRepository<UserRoles> _repoUR;
        private IRepository<Role> _repoR;
        private IRepository<AdvertiseAvailableVisitDays> _repoAv;
        private IRepository<AdvertiseVisitRequests> _repoReq;
        ILogger<AdvertiseService> _logger;
        //private readonly IJwtService _jwtService;

        public AdvertiseService(ILogger<AdvertiseService> logger, IRepository<AdvertiseVisitRequests> req, IRepository<AdvertiseAvailableVisitDays> repoav, IRepository<User> repository, ApplicationDbContext context,/* IJwtService jwtService,*/ IRepository<UserRoles> repoUR, IRepository<Role> repoR, IRepository<UserAdvertises> repoAd) : base(logger)
        {
            _repo = repository;
            //_jwtService = jwtService;
            _repoUR = repoUR;
            _repoR = repoR;
            _repoAd = repoAd;
            _repoAv = repoav;
            _repoReq = req;
        }

        #region public
        public async Task<ServiceResult> GetAdveriseForShow(int advertiseId)
        {
            try
            {
                var ua = await _repoAd.TableNoTracking
                .Where(u => u.Id == advertiseId && !u.IsDelete && u.IsConfirm)
                .FirstOrDefaultAsync();

                if (ua == null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.AdveriseNotFound, null);///

                if (!ua.ForSale)
                {

                    var result = new RentAdvertiseDto
                    {

                        AdvertiserName = ua.AdvertiserName,
                        AdvertiserNumber = ua.AdvertiserNumber,
                        AdvertiseText = ua.AdvertiseText,
                        HomeAddress = ua.HomeAddress,
                        RoomCount = ua.RoomCount,
                        Meterage = ua.Meterage,
                        ForSale = ua.ForSale,
                        DespositPrice = ua.DespositPrice,
                        RentPrice = ua.RentPrice,
                        HasBalcony = ua.HasBalcony,
                        HasElevator = ua.HasElevator,
                        HasWarehouse = ua.HasWarehouse,
                        HasGarage = ua.HasGarage,
                        BuildingType = ua.BuildingType,
                        Description = ua.Description,
                        CreatedDate = ua.CreatedDate,

                    };

                    return Ok(result);
                }

                else
                {
                    var result = new SaleAdvertiseDto
                    {

                        AdvertiserName = ua.AdvertiserName,
                        AdvertiserNumber = ua.AdvertiserNumber,
                        AdvertiseText = ua.AdvertiseText,
                        HomeAddress = ua.HomeAddress,
                        RoomCount = ua.RoomCount,
                        Meterage = ua.Meterage,
                        ForSale = ua.ForSale,
                        PricePerMeter = ua.PricePerMeter,
                        TotalPrice = ua.TotalPrice,
                        HasBalcony = ua.HasBalcony,
                        HasElevator = ua.HasElevator,
                        HasWarehouse = ua.HasWarehouse,
                        HasGarage = ua.HasGarage,
                        BuildingType = ua.BuildingType,
                        Description = ua.Description,
                        CreatedDate = ua.CreatedDate,

                    };

                    return Ok(result);
                }

            }

            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }

        public async Task<ServiceResult> GetAllAdvertises(int pageId = 1, string advertiseText = "", string homeAddress = "", string orderBy = "date", string saleType = "all", long startprice = 0, long endprice = 0, long startrentprice = 0, long endrentprice = 0)
        {
            try
            {
                IQueryable<UserAdvertises> result = _repoAd.Table.Where(u => !u.IsDelete && u.IsConfirm);

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
                switch (orderBy)
                {
                    case "date":
                        {
                            result = result.OrderByDescending(c => c.CreatedDate);
                            break;
                        }
                }

                switch (saleType)
                {
                    case "all":
                        {
                            break;
                        }

                    case "rent":
                        {

                            if (startrentprice >= 0 && endrentprice >= 0 && endrentprice > startrentprice)
                            {
                                result = result.Where(c => c.RentPrice >= startrentprice && c.RentPrice <= endrentprice && !c.ForSale);

                            }
                            else
                            {
                                return BadRequest(ErrorCodeEnum.BadRequest, Resource.EnterParametersCorrectlyAndCompletely, null);///
                            }
                            break;

                        }

                    case "sale":
                        {
                            if (startprice >= 0 && endprice >= 0 && endprice > startprice)
                            {
                                result = result.Where(c => c.TotalPrice >= startprice && c.TotalPrice <= endprice && c.ForSale);
                            }
                            else
                            {
                                return BadRequest(ErrorCodeEnum.BadRequest, Resource.EnterParametersCorrectlyAndCompletely, null);///
                            }
                            break;
                        }
                }
                int take = 10;
                int skip = (pageId - 1) * take;


                int pagecount = result.Select(c => new UserAdvertisesForHomePage()
                {

                    AdvertiseId = c.Id,
                    AdvertiseTitle = c.AdvertiseText,
                    AdvertiserName = c.AdvertiserName,
                    AdvertiserPhone = c.AdvertiserNumber,
                    ForSale = c.ForSale,
                    DespositPrice = c.DespositPrice,
                    RentPrice = c.RentPrice,
                    Price = c.TotalPrice

                }).Count() / take;

                var query = result.Select(c => new UserAdvertisesForHomePage()
                {
                    AdvertiseId = c.Id,
                    AdvertiseTitle = c.AdvertiseText,
                    AdvertiserName = c.AdvertiserName,
                    AdvertiserPhone = c.AdvertiserNumber,
                    ForSale = c.ForSale,
                    DespositPrice = c.DespositPrice,
                    RentPrice = c.RentPrice,
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

        public async Task<ServiceResult> GetAdvertiseAvailableVisitDays(int advertiseId)
        {
            try
            {
                ValidateModel(advertiseId);


                var result = _repoAv.TableNoTracking
                    .Where(u => u.AdvertiseId == advertiseId)
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }

        public async Task<ServiceResult> RequestForAdvertiseVisit(int dayOfWeek, int advertiseId, int userId, string fullName)
        {
            try
            {
                ValidateModel(advertiseId);

                var result = _repoAv.TableNoTracking
                    .Any(u => u.AdvertiseId == advertiseId && u.DayOfWeek == (DaysOfWeek)dayOfWeek);

                if (!result)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.AdvertiseDayNotMatch, null);///

                var userCheck = _repoAd.TableNoTracking.Any(u => u.Id == advertiseId && u.UserId == userId);

                if (userCheck)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.UserRequestError, null);///

                var req = new AdvertiseVisitRequests
                {
                    AdvertiseId = advertiseId,
                    DayOfWeek = (DaysOfWeek)dayOfWeek,
                    FullNameOfUser = fullName,
                    UserIdOfUser = userId,
                    IsConfirm = false,
                    IsDelete = false
                };

                _repoReq.Add(req);

                return Ok(req);
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
