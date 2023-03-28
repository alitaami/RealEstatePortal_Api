using Common.Resources;
using Common.Utilities;
using Data;
using Data.Repositories;
using Entities.Base;
using Entities.Common.Dtos;
using Entities.Common.ViewModels;
using Entities.Models.User;
using Entities.Models.User.Advertises;
using Entities.Models.User.Roles;
using EstateAgentApi.Services.Base;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Interfaces.Services
{
    public class AdminService : ServiceBase<AdminService>, IAdminService
    {
        private IRepository<User> _repo;
        private IRepository<UserRoles> _repoUR;
        private IRepository<Role> _repoR;
        private IRepository<UserAdvertises> _repoAd;
        private IRepository<AdvertiseAvailableVisitDays> _repoAv;
        private IRepository<AdvertiseVisitRequests> _repoReq;

        public AdminService(ILogger<AdminService> logger, IRepository<AdvertiseVisitRequests> repoReq, IRepository<UserAdvertises> repoad, IRepository<User> repository, IRepository<UserRoles> repoUR, IRepository<AdvertiseAvailableVisitDays> repoav, IRepository<Role> repoR) : base(logger)
        {
            _repo = repository;
            _repoUR = repoUR;
            _repoR = repoR;
            _repoAd = repoad;
            _repoAv = repoav;
            _repoReq = repoReq;
        }

        #region Advertise Section
        public async Task<ServiceResult> GetAllAdvertises(int pageId = 1, string advertiseText = "", string homeAddress = "", string orderBy = "date", string saleType = "all", long startprice = 0, long endprice = 0, long startrentprice = 0, long endrentprice = 0)
        {
            try
            {
                IQueryable<UserAdvertises> result = _repoAd.Table.Where(u => !u.IsDelete);

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
        public async Task<ServiceResult> UpdateAdvertise(int advertiseId, UserAdvertiseViewModelForAdmin ua, CancellationToken cancellationToken)
        {
            try
            {
                var uAd = await _repoAd.GetByIdAsync(cancellationToken, advertiseId);

                if (uAd is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

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
                    uAd.IsDelete = ua.IsDelete;
                    uAd.IsConfirm = ua.IsConfirm;
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
                    uAd.IsDelete = ua.IsDelete;
                    uAd.IsConfirm = ua.IsConfirm;
                }

                _repoAd.Update(uAd);

                if (uAd.ForSale)
                {

                    var result = new UserAdvertiseDto.SaleAdvertiseDtoForAdmin
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
                        CreatedDate = uAd.CreatedDate,
                        IsDelete = uAd.IsDelete,
                        IsConfirm = uAd.IsConfirm

                    };

                    return Ok(result);
                }
                else
                {
                    var result = new UserAdvertiseDto.RentAdvertiseDtoForAdmin
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
                        CreatedDate = uAd.CreatedDate,
                        IsDelete = uAd.IsDelete,
                        IsConfirm = uAd.IsConfirm
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
        public async Task<ServiceResult> CreateAdvertise(UserAdvertiseViewModelForAdmin ua, int userId, CancellationToken cancellationToken)
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
                    u.IsConfirm = ua.IsConfirm;
                    u.IsDelete = ua.IsDelete;
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
                    u.IsConfirm = ua.IsConfirm;
                    u.IsDelete = ua.IsDelete;
                }

                await _repoAd.AddAsync(u, cancellationToken);

                if (u.ForSale)
                {

                    var result = new UserAdvertiseDto.SaleAdvertiseDtoForAdmin
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
                        CreatedDate = u.CreatedDate,
                        IsConfirm = ua.IsConfirm,
                        IsDelete = ua.IsDelete
                    };

                    return Ok(result);
                }
                else
                {
                    var result = new UserAdvertiseDto.RentAdvertiseDtoForAdmin
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
                        CreatedDate = u.CreatedDate,
                        IsConfirm = ua.IsConfirm,
                        IsDelete = ua.IsDelete
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
        public async Task<ServiceResult> AddRolesToUser(List<int> SelectedRoles, int userid)
        {
            try
            {
                ValidateModel(SelectedRoles);

                foreach (int roleId in SelectedRoles)
                {

                    var r = _repoR.TableNoTracking.Any(r => r.Id == roleId && !r.IsDelete);

                    if (!r)
                        return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                    if (!await UserExist(userid))
                        return BadRequest(ErrorCodeEnum.BadRequest, Resource.NotFound, null);///

                    if (_repoUR.TableNoTracking.Any(r => r.RoleId == roleId && r.UserId == userid))
                        return BadRequest(ErrorCodeEnum.BadRequest, Resource.UserRoleExist, null);///

                    var role = new UserRoles
                    {
                        RoleId = roleId,
                        UserId = userid

                    };

                    _repoUR.Add(role);
                }
                return Ok();

            }

            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }
        public async Task<ServiceResult> EditRolesUser(List<int> SelectedRoles, int userid)
        {
            try
            {
                ValidateModel(SelectedRoles);

                _repoUR.Entities
                    .Where(r => r.UserId == userid).ToList()
                    .ForEach(r => _repoUR.Delete(r));

                #region add new roles

                foreach (int roleId in SelectedRoles)
                {

                    var r = _repoR.TableNoTracking.Any(r => r.Id == roleId && !r.IsDelete);

                    if (!r)
                        return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                    if (!await UserExist(userid))
                        return BadRequest(ErrorCodeEnum.BadRequest, Resource.NotFound, null);///

                    if (_repoUR.TableNoTracking.Any(r => r.RoleId == roleId && r.UserId == userid))
                        return BadRequest(ErrorCodeEnum.BadRequest, Resource.UserRoleExist, null);///

                    var role = new UserRoles
                    {
                        RoleId = roleId,
                        UserId = userid

                    };

                    _repoUR.Add(role);
                }
                #endregion

                return Ok();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }
        //public async Task<ServiceResult> GetUserRoles(int userid)
        //{
        //    try
        //    {
        //        ValidateModel(userid);

        //        var res = _repoUR.TableNoTracking
        //            .Where(u => u.UserId == userid).ToList();

        //        if (res is null)
        //            return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, null, null);

        //        return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

        //    }
        //}
        public async Task<ServiceResult> GetRoles()
        {
            try
            {
                var res = _repoR.TableNoTracking.ToList().Where(r => !r.IsDelete);

                if (res is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
        public async Task<ServiceResult> AddRoles(RoleViewModel role)
        {
            try
            {
                if (await RoleExistForCerate(role.Name))
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.RoleExist, null);///

                var res = new Role
                {
                    Name = role.Name,
                    Description = role.Description,
                    IsDelete = role.IsDelete
                    
                };

                _repoR.Add(res);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }

        }
        public async Task<ServiceResult> EditRoles(int roleId, RoleViewModel role)
        {
            try
            {
                var r = await _repoR.Entities.Where(r => r.Id == roleId && !r.IsDelete).FirstOrDefaultAsync();

                if (r is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///
                  
                if (await RoleExistForEdit(role.Name, roleId))
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.RoleExist, null);///

                r.Name = role.Name;
                r.Description = role.Description;
                r.IsDelete = role.IsDelete;

                _repoR.Update(r);

                return Ok(r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }

        }
        public async Task<ServiceResult> GetAllUsers(int pageId = 1, string fullName = "", string phoneNumber = "", string email = "")
        {
            try
            {
                IQueryable<User> result = _repo.TableNoTracking;

                if (result is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                if (!string.IsNullOrEmpty(fullName))
                {
                    result = result.Where(r => r.FullName.Contains(fullName));

                }
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    result = result.Where(r => r.PhoneNumber.Contains(phoneNumber));

                }
                if (!string.IsNullOrEmpty(email))
                {
                    result = result.Where(r => r.Email.Contains(email));

                }
                int take = 10;
                int skip = (pageId - 1) * take;


                int pagecount = result.Select(c => new EstateUserViewModel()
                {

                    UserName = c.UserName,
                    Age = c.Age,
                    Email = c.Email,
                    FullName = c.FullName,
                    PhoneNumber = c.PhoneNumber,
                    EstatePhoneNumber = c.EstatePhoneNumber,
                    EstateAddress = c.EstateAddress,
                    EstateCode = c.EstateCode,
                    LastLoginDate = c.LastLoginDate


                }).Count() / take;

                var query = result.Select(c => new EstateUserViewModel()
                {
                    UserName = c.UserName,
                    Age = c.Age,
                    Email = c.Email,
                    FullName = c.FullName,
                    PhoneNumber = c.PhoneNumber,
                    EstatePhoneNumber = c.EstatePhoneNumber,
                    EstateAddress = c.EstateAddress,
                    EstateCode = c.EstateCode,
                    LastLoginDate = c.LastLoginDate

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
        public async Task<ServiceResult> CreateUser(UserViewModelFromAdmin user)
        {
            try
            {
                ValidateModel(user);

                var checkDb = _repo.TableNoTracking
                   .Where(u => u.UserName == user.UserName || u.Email == user.Email || u.PhoneNumber == user.PhoneNumber || u.EstateCode == user.EstateCode);

                if (checkDb.Any())
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.AlreadyExists2, null);///

                var PasswordHash = SecurityHelper.GetSha256Hash(user.Password);

                if (user.IsEstateConsultant)
                {
                    var u = new User
                    {
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        PasswordHash = PasswordHash,
                        FullName = user.FullName,
                        EstateCode = user.EstateCode,
                        Age = user.Age,
                        Email = user.Email,
                        IsActive = user.IsActive,
                        IsEstateConsultant = user.IsEstateConsultant,
                        EstateAddress = user.EstateAddress,
                        EstatePhoneNumber = user.EstatePhoneNumber,
                        LastLoginDate = DateTimeOffset.Now,
                    };
                    _repo.Add(u);
                }
                else
                {
                    var u = new User
                    {
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        PasswordHash = PasswordHash,
                        FullName = user.FullName,
                        EstateCode = null,
                        Age = user.Age,
                        Email = user.Email,
                        IsActive = user.IsActive,
                        IsEstateConsultant = user.IsEstateConsultant,
                        EstateAddress = null,
                        EstatePhoneNumber = null,
                        LastLoginDate = DateTimeOffset.Now,
                    };
                    _repo.Add(u);
                }


                return Ok();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
        public async Task<ServiceResult> EditUser(EditUserViewModelFromAdmin user, int userId)
        {
            try
            {
                ValidateModel(user);

                var r = await _repo.Entities.Where(r => r.Id == userId).FirstOrDefaultAsync();

                if (r is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                //if (await _repo.TableNoTracking.AnyAsync(u => u.UserName == user.UserName || u.PhoneNumber == user.PhoneNumber|| u.Email == user.Email || u.EstateCode == user.EstateCode))
                //    return BadRequest(ErrorCodeEnum.BadRequest, Resource.AlreadyExists2, null);///

                var PasswordHash = SecurityHelper.GetSha256Hash(user.Password);

                if (user.IsEstateConsultant)
                {
                    //r.UserName = user.UserName;
                    //r.PhoneNumber = user.PhoneNumber;
                    r.PasswordHash = PasswordHash;
                    r.FullName = user.FullName;
                    //r.EstateCode = user.EstateCode;
                    r.Age = user.Age;
                    //r.Email = user.Email;
                    r.IsActive = user.IsActive;
                    r.IsEstateConsultant = user.IsEstateConsultant;
                    r.EstateAddress = user.EstateAddress;
                    r.EstatePhoneNumber = user.EstatePhoneNumber;
                    r.LastLoginDate = DateTimeOffset.Now;

                    _repo.Update(r);
                }
                else
                {
                    //r.UserName = user.UserName;
                    //r.PhoneNumber = user.PhoneNumber;
                    r.PasswordHash = PasswordHash;
                    r.FullName = user.FullName;
                    //r.EstateCode = null;
                    r.Age = user.Age;
                    //r.Email = user.Email;
                    r.IsActive = user.IsActive;
                    r.IsEstateConsultant = user.IsEstateConsultant;
                    r.EstateAddress = null;
                    r.EstatePhoneNumber = null;
                    r.LastLoginDate = DateTimeOffset.Now;

                    _repo.Update(r);
                }

                return Ok();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
        #endregion

        #region most used methods
        public async Task<bool> RoleExistForEdit(string name,int roleId)
        {

            var roleExist = _repoR.TableNoTracking.Where(r => r.Id != roleId).Any(r => r.Name == name && !r.IsDelete);

            if (roleExist)
                return true;///

            else
                return false;
        }
        public async Task<bool> RoleExistForCerate(string name)
        {

            var roleExist = _repoR.TableNoTracking.Any(r => r.Name == name && !r.IsDelete);

            if (roleExist)
                return true;///

            else
                return false;
        }
        public async Task<bool> UserExist(int userId)
        {

            var userExist = _repo.TableNoTracking.Any(r => r.Id == userId);

            if (userExist)
                return true;///

            else
                return false;
        }
        #endregion
    }

}
