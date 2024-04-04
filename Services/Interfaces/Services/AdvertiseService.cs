using Common;
using Common.Resources;
using Common.Utilities;
using Data;
using Data.Repositories;
using Entities.Base;
using Entities.Common.Dtos;
using Entities.Common.Enums;
using Entities.Common.ViewModels;
using Entities.Models.Advertises;
using Entities.Models.Roles;
using Entities.Models.User;
using EstateAgentApi.Services.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Runtime.Caching;
using System.Text;
using static Entities.Common.Dtos.UserAdvertiseDto;

namespace Services.Interfaces.Services
{
    public class AdvertiseService : ServiceBase<AdvertiseService>, IAdvertiseService
    {
        private IRepository<User> _repo;
        private IRepository<UserAdvertises> _repoAd;
        private IRepository<UserRoles> _repoUR;
        private IRepository<Entities.Models.Roles.Role> _repoR;
        private IRepository<AdvertiseAvailableVisitDays> _repoAv;
        private IRepository<AdvertiseVisitRequests> _repoReq;
        private IRepository<AdvertiseImages> _repoIm;
        ILogger<AdvertiseService> _logger;
        private readonly IUserService _user;
        private readonly IMemoryService _memory;
        private readonly MemoryCache _cache;
        public AdvertiseService(IMemoryService memory, ILogger<AdvertiseService> logger, IRepository<AdvertiseImages> repoIm, IUserService user, IRepository<AdvertiseVisitRequests> req, IRepository<AdvertiseAvailableVisitDays> repoav, IRepository<User> repository, ApplicationDbContext context,/* IJwtService jwtService,*/ IRepository<UserRoles> repoUR, IRepository<Entities.Models.Roles.Role> repoR, IRepository<UserAdvertises> repoAd) : base(logger)
        {
            _repo = repository;
            //_jwtService = jwtService;
            _repoUR = repoUR;
            _repoR = repoR;
            _repoAd = repoAd;
            _repoAv = repoav;
            _repoReq = req;
            _user = user;
            _repoIm = repoIm;
            _memory = memory;
            _logger = logger;
            _cache = MemoryCache.Default;
        }

        #region public
        public async Task<ServiceResult> GetAdveriseForShow(int advertiseId)
        {
            try
            {
                var cachedResult = await _memory
                    .GetAsync<SaleAdvertiseDto>(KeysForCache
                    .getAdvertiseForShowKey(advertiseId));

                if (cachedResult != null)
                {
                    return Ok(cachedResult);
                }

                var ua = await _repoAd.TableNoTracking
                    .Where(u => u.Id == advertiseId && !u.IsDelete && u.IsConfirm)
                    .FirstOrDefaultAsync();

                if (ua == null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.AdveriseNotFound, null);

                if (!ua.ForSale)
                {
                    var result = new RentAdvertiseDto
                    {
                        AdvertiserName = ua.AdvertiserName,
                        AdvertiserNumber = ua.AdvertiserNumber,
                        AdvertiseText = ua.AdvertiseText,
                        Address = ua.Address,
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
                        CreatedDate = ua.CreatedDate
                    };

                    return Ok(result); // Return the result
                }
                else
                {
                    var result = new SaleAdvertiseDto
                    {
                        AdvertiserName = ua.AdvertiserName,
                        AdvertiserNumber = ua.AdvertiserNumber,
                        AdvertiseText = ua.AdvertiseText,
                        Address = ua.Address,
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
                        CreatedDate = ua.CreatedDate
                    };

                    await _memory
                        .SetAsync(KeysForCache
                        .getAdvertiseForShowKey(advertiseId), result, TimeSpan.FromMinutes(30));

                    return Ok(result); // Return the result
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
                //using In Memory Cache
                string key = "AllAdvertises";
                IQueryable<UserAdvertises> result;

                if (_cache.Contains(key))
                {
                    result = (IQueryable<UserAdvertises>)_cache.Get(key);
                }
                else
                {
                    result = _repoAd.Table.Where(u => !u.IsDelete && u.IsConfirm);

                    if (result is null)
                        return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                    _cache.Set(key, result, DateTimeOffset.Now.AddMinutes(10));
                }
                if (!string.IsNullOrEmpty(advertiseText))
                {
                    result = result.Where(r => r.AdvertiseText.Contains(advertiseText));

                }
                if (!string.IsNullOrEmpty(homeAddress))
                {
                    result = result.Where(r => r.Address.Contains(homeAddress));

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
        public async Task<ServiceResult> GetAdvertiseImages(int advertiseId)
        {
            var images = _repoIm.TableNoTracking
                .Where(u => u.AdvertiseId == advertiseId).ToList();

            if (images.Count == 0 && images is null)
                return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);

            var showImagesList = new List<AdvertiseImagesDto>();

            foreach (var image in images)
            {
                var showImages = new AdvertiseImagesDto
                {
                    FileId = image.Id,
                    FileName = image.FileName,
                    FilePath = image.FilePath
                };
                showImagesList.Add(showImages);
            }

            if (showImagesList is null)
                return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);

            return Ok(showImagesList);
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
        public async Task<ServiceResult> RequestForAdvertiseVisit(DateTimeOffset dayOfWeek, int advertiseId, int userId, string fullName)
        {
            try
            {
                ValidateModel(advertiseId);

                #region conditions
                var result = _repoAv.TableNoTracking
                    .Any(u => u.AdvertiseId == advertiseId && u.AvailableVisitDay == dayOfWeek);

                if (!result)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.AdvertiseDayNotMatch, null);///

                var userCheck = _repoAd.TableNoTracking
                    .Any(u => u.Id == advertiseId && u.UserId == userId && !u.IsDelete && u.IsConfirm);

                if (userCheck)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.UserRequestError, null);///

                var requestExist = _repoReq.TableNoTracking
                    .Any(u => u.UserIdOfUser == userId && u.AdvertiseId == advertiseId && !u.IsDelete);

                if (requestExist)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.RequestExists, null);///
                #endregion

                var req = new AdvertiseVisitRequests
                {
                    AdvertiseId = advertiseId,
                    AvailableVisitDay = dayOfWeek,
                    FullNameOfUser = fullName,
                    UserIdOfUser = userId,
                    IsConfirm = false,
                    IsDelete = false
                };

                _repoReq.Add(req);

                //Send mail to advertiser

                #region send activation email            
                var email = await GetAdvertiserEmail(req.AdvertiseId);
                string body = Resource.EmailSubject2 + " ** " + fullName + " ** " + Resource.EmailSubject2_1;

                await SendMail.SendAsync(email, Resource.RequestVisit, body);
                #endregion

                var res = new AdvertiseVisitRequestsDto
                {
                    AdvertiseId = req.AdvertiseId,
                    AvailableVisitDay = req.AvailableVisitDay,
                    FullNameOfUser = req.FullNameOfUser,
                    IsConfirm = req.IsConfirm

                };

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }

        #endregion

        #region most used methods
        public async Task<string> GetUserFullname(int userId)
        {

            ValidateModel(userId);

            var result = await _repo.Entities
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (result is null)
                return null;///

            var fullName = result.FullName;

            return fullName;

        }
        public async Task<string> GetAdvertiserEmail(int advertiseId)
        {

            ValidateModel(advertiseId);

            var result = await _repoAd.Entities
                .Where(u => u.Id == advertiseId)
                .FirstOrDefaultAsync();

            if (result is null)
                return null;///

            var userId = result.UserId;

            var user = await _repo.Entities
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user is null)
                return null;///

            return user.Email;

        }

        #endregion

    }
}
