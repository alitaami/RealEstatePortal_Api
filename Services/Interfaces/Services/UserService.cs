using Common.Resources;
using Common.Utilities;
using Data;
using Data.Repositories;
using Entities.Base;
using Entities.Common.Dtos;
using Entities.Common.ViewModels;
using Entities.Models.User;
using EstateAgentApi.Services.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.Services
{
    public class UserService : ServiceBase<UserService>, IUserService
    {
        private IRepository<User> _repo;
        private IRepository<UserRoles> _repoUR;
        private IRepository<Role> _repoR;
        private IRepository<UserAdvertises> _repoAd;
        private readonly IJwtService _jwtService;
        public UserService(ILogger<UserService> logger, IRepository<UserAdvertises> repoad, IRepository<User> repository, IJwtService jwtService, IRepository<UserRoles> repoUR, IRepository<Role> repoR) : base(logger)
        {
            _repo = repository;
            _repoUR = repoUR;
            _repoR = repoR;
            _jwtService = jwtService;
            _repoAd = repoad;
        }

        #region User Panel


        public async Task<ServiceResult> UpdateEstateAgentInfo(int userId, EstateAgentPanelViewModel user, CancellationToken cancellationToken)
        {
            try
            {
                ValidateModel(user);

                var u = await _repo.GetByIdAsync(cancellationToken, userId);

                if (u is null)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.GeneralErrorTryAgain, null);///

                var userRole = await _repoUR.TableNoTracking.Where(u => u.UserId == userId).FirstOrDefaultAsync();

                if (userRole.RoleId == 2)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.GeneralErrorTryAgain, null);///


                if (user.OldPassword == "" || user.NewPassword == "" || user.RePassword == "")
                {
                    u.Age = user.Age;
                    u.EstateAddress = user.EstateAddress;
                    u.EstateCode = user.EstateCode;
                    u.EstatePhoneNumber = user.EstatePhoneNumber;
                    u.FullName = user.FullName;
                }

                else
                {
                    var hashOld = SecurityHelper.GetSha256Hash(user.OldPassword);
                    var hashNewPass = SecurityHelper.GetSha256Hash(user.NewPassword);

                    if (hashOld != u.PasswordHash)
                        return BadRequest(ErrorCodeEnum.BadRequest, Resource.PasswordDoesntMatch, null);///

                    else
                    {
                        if (user.NewPassword == "")
                            return BadRequest(ErrorCodeEnum.BadRequest, Resource.EnterParametersCorrectlyAndCompletely, null);///

                        u.PasswordHash = hashNewPass;
                        u.Age = user.Age;
                        u.EstateAddress = user.EstateAddress;
                        u.EstateCode = user.EstateCode;
                        u.EstatePhoneNumber = user.EstatePhoneNumber;
                        u.FullName = user.FullName;
                    }

                }

                _repo.Update(u);

                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }

        public async Task<ServiceResult> GetUserInfo(int userId, CancellationToken cancellationToken)
        {
            try
            {
                ValidateModel(userId);

                var u = await _repo.TableNoTracking
                      .Where(u => u.Id == userId)
                      .FirstOrDefaultAsync();

                if (u == null)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.GeneralErrorTryAgain, null);///

                if (u.IsEstateConsultant)
                {
                    var agent = new EstateAgentDto
                    {
                        UserName = u.UserName,
                        Email = u.Email,
                        Age = u.Age,
                        PhoneNumber = u.PhoneNumber,
                        FullName = u.FullName,
                        EstateAddress = u.EstateAddress,
                        EstatePhoneNumber = u.EstatePhoneNumber,
                        EstateCode = u.EstateCode,
                        LastLoginDate = u.LastLoginDate

                    };

                    return Ok(agent);
                }
                else
                {

                    var user = new UserDto
                    {
                        UserName = u.UserName,
                        Email = u.Email,
                        Age = u.Age,
                        PhoneNumber = u.PhoneNumber,
                        FullName = u.FullName,
                        LastLoginDate = u.LastLoginDate

                    };

                    return Ok(user);
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }

        public async Task<ServiceResult> UpdateUserInfo(int userId, UserPanelViewModel user, CancellationToken cancellationToken)
        {
            try
            {
                ValidateModel(user);

                var u = await _repo.GetByIdAsync(cancellationToken, userId);

                if (u is null)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.GeneralErrorTryAgain, null);///

                var userRole = await _repoUR.TableNoTracking.Where(u => u.UserId == userId).FirstOrDefaultAsync();

                if (userRole.RoleId == 1)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.GeneralErrorTryAgain, null);///


                if (user.OldPassword == "" || user.NewPassword == "" || user.RePassword == "")
                {
                    u.Age = user.Age;
                    u.FullName = user.FullName;
                }

                else
                {
                    var hashOld = SecurityHelper.GetSha256Hash(user.OldPassword);
                    var hashNewPass = SecurityHelper.GetSha256Hash(user.NewPassword);

                    if (hashOld != u.PasswordHash)
                        return BadRequest(ErrorCodeEnum.BadRequest, Resource.PasswordDoesntMatch, null);///

                    else
                    {
                        if (user.NewPassword == "")
                            return BadRequest(ErrorCodeEnum.BadRequest, Resource.EnterParametersCorrectlyAndCompletely, null);///

                        u.PasswordHash = hashNewPass;
                        u.Age = user.Age;
                        u.FullName = user.FullName;
                    }
                }

                _repo.Update(u);

                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }

        #endregion

        #region  Advertises Of User
        public async Task<ServiceResult> CreateAdvertise(UserAdvertiseViewModel ua, int userId, CancellationToken cancellationToken)
        {
            try
            {
                ValidateModel(ua);

                UserAdvertises u = new UserAdvertises();

                var ud = await _repoAd.TableNoTracking
                       .Where(u => u.AdvertiseText == ua.AdvertiseText)
                       .FirstOrDefaultAsync();

                if (ud != null)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.AdExists, null);///

                //var EstateAgent = await _repo.GetByIdAsync(cancellationToken, userId);

                if (ua.ForSale)
                {
                    u.UserId = userId;
                    u.AdvertiseText = ua.AdvertiseText;
                    u.AdvertiserName = ua.AdvertiserName;
                    u.AdvertiserNumber = ua.AdvertiserNumber;
                    u.HomeAddress = ua.HomeAddress;
                    u.Meterage = ua.Meterage;
                    u.ForSale = true;
                    u.PricePerMeter = ua.PricePerMeter;
                    u.TotalPrice = ua.Meterage * ua.PricePerMeter;
                    u.RoomCount = ua.RoomCount;
                    u.DespositPrice = null;
                    u.RentPrice = null;
                    u.HasBalcony = ua.HasBalcony;
                    u.HasElevator = ua.HasElevator;
                    u.HasWarehouse = ua.HasWarehouse;
                    u.HasGarage = ua.HasGarage;
                    u.Description = ua.Description;
                    u.CreatedDate = DateTimeOffset.Now;
                }
                else
                {
                    u.UserId = userId;
                    u.AdvertiseText = ua.AdvertiseText;
                    u.AdvertiserName = ua.AdvertiserName;
                    u.AdvertiserNumber = ua.AdvertiserNumber;
                    u.HomeAddress = ua.HomeAddress;
                    u.Meterage = ua.Meterage;
                    u.ForSale = false;
                    u.PricePerMeter = null;
                    u.TotalPrice = null;
                    u.RoomCount = ua.RoomCount;
                    u.DespositPrice = ua.DespositPrice;
                    u.RentPrice = ua.RentPrice;
                    u.HasBalcony = ua.HasBalcony;
                    u.HasElevator = ua.HasElevator;
                    u.HasWarehouse = ua.HasWarehouse;
                    u.HasGarage = ua.HasGarage;
                    u.Description = ua.Description;
                    u.CreatedDate = DateTimeOffset.Now;
                }

                await _repoAd.AddAsync(u, cancellationToken);

                if (u.ForSale)
                {

                    var result = new UserAdvertiseDto.SaleAdvertiseDto
                    {
                        AdvertiserName = ua.AdvertiserName,
                        AdvertiserNumber = ua.AdvertiserNumber,
                        AdvertiseText = ua.AdvertiseText,
                        HomeAddress = ua.HomeAddress,
                        Meterage = ua.Meterage,
                        PricePerMeter = ua.PricePerMeter,
                        TotalPrice = ua.Meterage * ua.PricePerMeter,
                        RoomCount = ua.RoomCount,
                        BuildingType = ua.BuildingType,
                        HasBalcony = ua.HasBalcony,
                        HasElevator = ua.HasElevator,
                        HasWarehouse = ua.HasWarehouse,
                        HasGarage = ua.HasGarage,
                        Description = ua.Description,
                        CreatedDate = u.CreatedDate
                    };

                    return Ok(result);
                }
                else
                {
                    var result = new UserAdvertiseDto.RentAdvertiseDto
                    {
                        AdvertiserName = ua.AdvertiserName,
                        AdvertiserNumber = ua.AdvertiserNumber,
                        AdvertiseText = ua.AdvertiseText,
                        HomeAddress = ua.HomeAddress,
                        Meterage = ua.Meterage,
                        DespositPrice = ua.DespositPrice,
                        RentPrice = ua.RentPrice,
                        RoomCount = ua.RoomCount,
                        BuildingType = ua.BuildingType,
                        HasBalcony = ua.HasBalcony,
                        HasElevator = ua.HasElevator,
                        HasWarehouse = ua.HasWarehouse,
                        HasGarage = ua.HasGarage,
                        Description = ua.Description,
                        CreatedDate = u.CreatedDate
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

        public async Task<ServiceResult> GetAllAdvertisesOfUser(int pageId = 1, string advertiseText = "", string homeAddress = "", string orderBy = "date", string saleType = "sale", long startprice = 0, long endprice = 0, long startrentprice = 0, long endrentprice = 0, int userId = 0)
        {
            try
            {
                if (userId == 0)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.GeneralErrorTryAgain, null);///

                IQueryable<UserAdvertises> result = _repoAd
                    .TableNoTracking
                    .Where(u => u.UserId == userId && !u.IsDelete);

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
                    case "rent":
                        {

                            if (startrentprice > 0 && endrentprice > 0 && endrentprice > startrentprice)
                            {
                                result = result.Where(c => c.RentPrice > startrentprice && c.RentPrice < endrentprice && !c.ForSale);

                            }
                            else
                            {
                                return BadRequest(ErrorCodeEnum.BadRequest, Resource.EnterParametersCorrectlyAndCompletely, null);///
                            }
                            break;
                        }

                    case "sale":
                        {
                            if (startprice > 0 && endprice > 0 && endprice > startprice)
                            {
                                result = result.Where(c => c.TotalPrice > startprice && c.TotalPrice < endprice && c.ForSale);
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

        public async Task<ServiceResult> UpdateAdvertiseOfUser(int advertiseId, int userId, UserAdvertiseViewModel ua, CancellationToken cancellationToken)
        {
            try
            {
                var uAd = await _repoAd.GetByIdAsync(cancellationToken, advertiseId);


                if (uAd is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                if (uAd.IsDelete)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.WrongAdvertise, null);///

                if (uAd.UserId != userId)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.WrongAdvertise, null);///

                if (ua.ForSale)
                {
                    uAd.AdvertiserName = ua.AdvertiserName;
                    uAd.AdvertiserNumber = ua.AdvertiserNumber;
                    uAd.AdvertiseText = ua.AdvertiseText;
                    uAd.HomeAddress = ua.HomeAddress;
                    uAd.Meterage = ua.Meterage;
                    uAd.PricePerMeter = ua.PricePerMeter;
                    uAd.TotalPrice = ua.Meterage * ua.PricePerMeter;
                    uAd.RoomCount = ua.RoomCount;
                    uAd.DespositPrice = null;
                    uAd.ForSale = true;
                    uAd.RentPrice = null;
                    uAd.HasBalcony = ua.HasBalcony;
                    uAd.HasElevator = ua.HasElevator;
                    uAd.HasWarehouse = ua.HasWarehouse;
                    uAd.HasGarage = ua.HasGarage;
                    uAd.Description = ua.Description;
                    uAd.CreatedDate = DateTimeOffset.Now;

                }
                else
                {
                    uAd.AdvertiserName = ua.AdvertiserName;
                    uAd.AdvertiserNumber = ua.AdvertiserNumber;
                    uAd.AdvertiseText = ua.AdvertiseText;
                    uAd.HomeAddress = ua.HomeAddress;
                    uAd.Meterage = ua.Meterage;
                    uAd.ForSale = false;
                    uAd.PricePerMeter = null;
                    uAd.TotalPrice = null;
                    uAd.DespositPrice = ua.DespositPrice;
                    uAd.RentPrice = ua.RentPrice;
                    uAd.RoomCount = ua.RoomCount;
                    uAd.HasBalcony = ua.HasBalcony;
                    uAd.HasElevator = ua.HasElevator;
                    uAd.HasWarehouse = ua.HasWarehouse;
                    uAd.HasGarage = ua.HasGarage;
                    uAd.Description = ua.Description;
                    uAd.CreatedDate = DateTimeOffset.Now;
                }

                _repoAd.Update(uAd);

                if (uAd.ForSale)
                {

                    var result = new UserAdvertiseDto.SaleAdvertiseDto
                    {
                        AdvertiserName = uAd.AdvertiserName,
                        AdvertiserNumber = uAd.AdvertiserNumber,
                        AdvertiseText = uAd.AdvertiseText,
                        HomeAddress = uAd.HomeAddress,
                        Meterage = uAd.Meterage,
                        PricePerMeter = uAd.PricePerMeter,
                        TotalPrice = uAd.TotalPrice,
                        RoomCount = uAd.RoomCount,
                        BuildingType = uAd.BuildingType,
                        HasBalcony = uAd.HasBalcony,
                        HasElevator = uAd.HasElevator,
                        HasWarehouse = uAd.HasWarehouse,
                        HasGarage = uAd.HasGarage,
                        Description = uAd.Description,
                        CreatedDate = uAd.CreatedDate
                    };

                    return Ok(result);
                }
                else
                {
                    var result = new UserAdvertiseDto.RentAdvertiseDto
                    {
                        AdvertiserName = uAd.AdvertiserName,
                        AdvertiserNumber = uAd.AdvertiserNumber,
                        AdvertiseText = uAd.AdvertiseText,
                        HomeAddress = uAd.HomeAddress,
                        Meterage = uAd.Meterage,
                        DespositPrice = uAd.DespositPrice,
                        RentPrice = uAd.RentPrice,
                        BuildingType = uAd.BuildingType,
                        RoomCount = uAd.RoomCount,
                        HasBalcony = uAd.HasBalcony,
                        HasElevator = uAd.HasElevator,
                        HasWarehouse = uAd.HasWarehouse,
                        HasGarage = uAd.HasGarage,
                        Description = uAd.Description,
                        CreatedDate = uAd.CreatedDate
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
        public async Task<ServiceResult> DeleteAdvertiseOfUser(int advertiseId, int userId, CancellationToken cancellationToken)
        {
            try
            {
                var uAd = await _repoAd.GetByIdAsync(cancellationToken, advertiseId);

                if (uAd is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                if (uAd.IsDelete)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.WrongAdvertise, null);///

                if (uAd.UserId != userId)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.WrongAdvertise, null);///

                uAd.IsDelete = true;

                _repoAd.Update(uAd);

                return Ok();

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
