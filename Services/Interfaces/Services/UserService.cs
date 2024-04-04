﻿using Common;
using Common.Resources;
using Common.Utilities;
using Data.Repositories;
using Entities.Base;
using Entities.Common.Dtos;
using Entities.Common.ViewModels;
using Entities.Models.Advertises;
using Entities.Models.Roles;
using Entities.Models.User;
using EstateAgentApi.Services.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Entities.Common.Dtos.UserAdvertiseDto;
using Path = System.IO.Path;

namespace Services.Interfaces.Services
{
    public class UserService : ServiceBase<UserService>, IUserService
    {
        private IRepository<User> _repo;
        private IRepository<UserRoles> _repoUR;
        private IRepository<Role> _repoR;
        private IRepository<UserAdvertises> _repoAd;
        private IRepository<AdvertiseAvailableVisitDays> _repoAv;
        private IRepository<AdvertiseVisitRequests> _repoReq;
        private IRepository<AdvertiseImages> _repoIm;
        private IWebHostEnvironment _env;
        private readonly IJwtService _jwtService;
        public UserService(ILogger<UserService> logger, IRepository<AdvertiseImages> repoIm, IWebHostEnvironment env, IRepository<AdvertiseVisitRequests> repoReq, IRepository<UserAdvertises> repoad, IRepository<User> repository, IJwtService jwtService, IRepository<UserRoles> repoUR, IRepository<AdvertiseAvailableVisitDays> repoav, IRepository<Role> repoR) : base(logger)
        {
            _repo = repository;
            _repoUR = repoUR;
            _repoR = repoR;
            _jwtService = jwtService;
            _repoAd = repoad;
            _repoAv = repoav;
            _repoReq = repoReq;
            _repoIm = repoIm;
            _env = env;
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

                #region RoleCheck
                /// we have authorize(Roles) attribute on method in controller; but i wrote this code for check IsDelete of Role
                var userRole = await _repoUR.TableNoTracking.Where(u => u.UserId == userId).FirstOrDefaultAsync();

                if (userRole == null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///
 
                if (userRole.RoleId == 2 || !!CheckRoleExistence(userRole.RoleId))
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.RoleDoesNotMatchUser, null);///
                #endregion

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

                #region RoleCheck
                /// we have authorize(Roles) attribute on method in controller; but i wrote this code for check IsDelete of Role
                var userRole = await _repoUR.TableNoTracking.Where(u => u.UserId == userId).FirstOrDefaultAsync();

                if (userRole == null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///
                 
                if (userRole.RoleId == 1 || !CheckRoleExistence(userRole.RoleId)) 
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.RoleDoesNotMatchUser, null);///
                #endregion

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
        public async Task<ServiceResult> CreateAdvertise([FromForm] UserAdvertiseViewModel ua, int userId, CancellationToken cancellationToken)
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
                    u.Address = ua.Address;
                    u.Meterage = ua.Meterage;
                    u.ForSale = true;
                    u.PricePerMeter = ua.PricePerMeter;
                    u.TotalPrice = ua.Meterage * ua.PricePerMeter;
                    u.RoomCount = ua.RoomCount;
                    u.DespositPrice = null;
                    u.RentPrice = null;
                    u.BuildingType = ua.BuildingType;
                    u.HasBalcony = ua.HasBalcony;
                    u.HasElevator = ua.HasElevator;
                    u.HasWarehouse = ua.HasWarehouse;
                    u.HasGarage = ua.HasGarage;
                    u.Description = ua.Description;
                    u.CreatedDate = DateTimeOffset.Now;
                    u.IsConfirm = false;
                }
                else
                {
                    u.UserId = userId;
                    u.AdvertiseText = ua.AdvertiseText;
                    u.AdvertiserName = ua.AdvertiserName;
                    u.AdvertiserNumber = ua.AdvertiserNumber;
                    u.Address = ua.Address;
                    u.Meterage = ua.Meterage;
                    u.ForSale = false;
                    u.PricePerMeter = null;
                    u.TotalPrice = null;
                    u.RoomCount = ua.RoomCount;
                    u.DespositPrice = ua.DespositPrice;
                    u.RentPrice = ua.RentPrice;
                    u.HasBalcony = ua.HasBalcony;
                    u.BuildingType = ua.BuildingType;
                    u.HasElevator = ua.HasElevator;
                    u.HasWarehouse = ua.HasWarehouse;
                    u.HasGarage = ua.HasGarage;
                    u.Description = ua.Description;
                    u.CreatedDate = DateTimeOffset.Now;
                    u.IsConfirm = false;
                }

                await _repoAd.AddAsync(u, cancellationToken);

                foreach (var file in ua.AdvertisePhotos)
                {
                    if (file is null)
                        return BadRequest(ErrorCodeEnum.BadRequest, Resource.ImageRequired, null);///

                    // Generate a unique file name and path
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/AdvertiseImages", fileName);

                    // Save the file to the wwwroot/images folder
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Add the files to the database
                    var advImages = new AdvertiseImages
                    {
                        FilePath = filePath,
                        FileName = fileName,
                        AdvertiseId = u.Id,
                        UserId = u.UserId
                    };

                    await _repoIm.AddAsync(advImages, cancellationToken);
                }

                if (u.ForSale)
                {
                    var result = new UserAdvertiseDto.SaleAdvertiseDto
                    {
                        AdvertiserName = ua.AdvertiserName,
                        AdvertiserNumber = ua.AdvertiserNumber,
                        AdvertiseText = ua.AdvertiseText,
                        Address = ua.Address,
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
                        Address = ua.Address,
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
        public async Task<ServiceResult> GetAllAdvertisesOfUser(int pageId = 1, string advertiseText = "", string homeAddress = "", string orderBy = "date", string saleType = "all", long startprice = 0, long endprice = 0, long startrentprice = 0, long endrentprice = 0, int userId = 0)
        {
            try
            {
                if (userId == 0)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.GeneralErrorTryAgain, null);///

                IQueryable<UserAdvertises> result = _repoAd
                    .TableNoTracking
                    .Where(u => u.UserId == userId && !u.IsDelete && u.IsConfirm);

                if (result is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

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

                int pagecount = result.Select(c => new UserAdvertiseDto.AdvertiseDto()
                {
                    AdvertiseId = c.Id,
                    AdvertiseText = c.AdvertiseText,
                    AdvertiserName = c.AdvertiserName,
                    AdvertiserNumber = c.AdvertiserNumber,
                    Meterage = c.Meterage,
                    PricePerMeter = c.PricePerMeter,
                    RoomCount = c.RoomCount,
                    ForSale = c.ForSale,
                    DespositPrice = c.DespositPrice,
                    RentPrice = c.RentPrice,
                    TotalPrice = c.TotalPrice,
                    BuildingType = c.BuildingType,
                    Address = c.Address,
                    HasBalcony = c.HasBalcony,
                    HasElevator = c.HasElevator,
                    HasGarage = c.HasGarage,
                    HasWarehouse = c.HasWarehouse,
                    Description = c.Description,
                    CreatedDate = c.CreatedDate

                }).Count() / take;

                var query = result.Select(c => new UserAdvertiseDto.AdvertiseDto()
                {
                    AdvertiseId = c.Id,
                    AdvertiseText = c.AdvertiseText,
                    AdvertiserName = c.AdvertiserName,
                    AdvertiserNumber = c.AdvertiserNumber,
                    Meterage = c.Meterage,
                    PricePerMeter = c.PricePerMeter,
                    RoomCount = c.RoomCount,
                    ForSale = c.ForSale,
                    DespositPrice = c.DespositPrice,
                    RentPrice = c.RentPrice,
                    TotalPrice = c.TotalPrice,
                    BuildingType = c.BuildingType,
                    Address = c.Address,
                    HasBalcony = c.HasBalcony,
                    HasElevator = c.HasElevator,
                    HasGarage = c.HasGarage,
                    HasWarehouse = c.HasWarehouse,
                    Description = c.Description,
                    CreatedDate = c.CreatedDate

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
        public async Task<ServiceResult> GetAdvertiseImagesOfUser(int advertiseId, int userId)
        {
            var advertise = _repoAd.TableNoTracking
                .Where(u=>u.Id == advertiseId)
                .FirstOrDefault();

            if(advertise is null)
                return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);

            var images = _repoIm.TableNoTracking
                .Where(u => u.AdvertiseId == advertiseId && u.UserId == userId).ToList();

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
        public async Task<ServiceResult> UpdateAdvertiseOfUser(int advertiseId, int userId, UserUpdateAdvertiseViewModel ua, CancellationToken cancellationToken)
        {
            try
            {
                var uAd = await _repoAd.GetByIdAsync(cancellationToken, advertiseId);


                if (uAd is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                if (uAd.IsDelete && !uAd.IsConfirm && uAd.UserId != userId)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.WrongAdvertise, null);///

                if (ua.ForSale)
                {
                    uAd.AdvertiserName = ua.AdvertiserName;
                    uAd.AdvertiserNumber = ua.AdvertiserNumber;
                    uAd.AdvertiseText = ua.AdvertiseText;
                    uAd.Address = ua.Address;
                    uAd.Meterage = ua.Meterage;
                    uAd.PricePerMeter = ua.PricePerMeter;
                    uAd.TotalPrice = ua.Meterage * ua.PricePerMeter;
                    uAd.RoomCount = ua.RoomCount;
                    uAd.DespositPrice = null;
                    uAd.ForSale = true;
                    uAd.RentPrice = null;
                    uAd.HasBalcony = ua.HasBalcony;
                    uAd.BuildingType = ua.BuildingType;
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
                    uAd.Address = ua.Address;
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
                    uAd.BuildingType = ua.BuildingType;
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
                        Address = uAd.Address,
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
                        Address = uAd.Address,
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
        public async Task<ServiceResult> UpdateAdvertiseImageOfUser(int fileId, int userId, [FromForm] AdvertiseImageViewModel image, CancellationToken cancellationToken)
        {
            try
            {
                var uIm = await _repoIm
                    .TableNoTracking
                    .Where(u => u.Id == fileId && u.UserId == userId).FirstOrDefaultAsync(cancellationToken);

                if (uIm is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                if (image.AdvertisePhotos is null)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.ImageRequired, null);///

                string ImageDeletepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/AdvertiseImages", uIm.FileName);
                if (File.Exists(ImageDeletepath))
                {
                    File.Delete(ImageDeletepath);
                }
                uIm.FileName = Guid.NewGuid().ToString() + Path.GetExtension(image.AdvertisePhotos.FileName);
                uIm.FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/AdvertiseImages", uIm.FileName);

                // Save the file to the wwwroot/images folder
                using (var stream = new FileStream(uIm.FilePath, FileMode.Create))
                {
                    await image.AdvertisePhotos.CopyToAsync(stream);
                }
                _repoIm.Update(uIm);

                var advImages = new AdvertiseImages
                {
                    FilePath = uIm.FilePath,
                    FileName = uIm.FileName,
                    AdvertiseId = uIm.Id,
                    UserId = uIm.UserId
                };
                return Ok(uIm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }
        public async Task<ServiceResult> DeleteAdvertiseImageOfUser(int fileId, int userId, CancellationToken cancellationToken)
        {
            try
            {
                var uIm = await _repoIm
                    .TableNoTracking
                    .Where(u => u.Id == fileId && u.UserId == userId).FirstOrDefaultAsync(cancellationToken);

                if (uIm is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                string ImageDeletepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/AdvertiseImages", uIm.FileName);
                if (File.Exists(ImageDeletepath))
                {
                    File.Delete(ImageDeletepath);
                }

                _repoIm.Delete(uIm);

                return Ok();
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

                if (uAd.IsDelete && !uAd.IsConfirm)
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
        public async Task<ServiceResult> CreateAdvertiseAvailableVisitDays(List<DateTimeOffset> SelectedDays, int advertiseId, int userId)
        {
            try
            {
                ValidateModel(advertiseId);

                if (!CheckUserHasThisAdvertise(advertiseId, userId))
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.WrongAdvertise, null);///

                foreach (var p in SelectedDays)
                {
                    //if (p < 1 || p > 7)
                    //    return BadRequest(ErrorCodeEnum.BadRequest, Resource.WrongDaySelected, null);///

                    var check = _repoAv.TableNoTracking
                        .Any(u => u.AvailableVisitDay == p && u.AdvertiseId == advertiseId);

                    if (check)
                        return BadRequest(ErrorCodeEnum.BadRequest, Resource.DaysExists, null);///

                    _repoAv.Add(new AdvertiseAvailableVisitDays
                    {
                        AvailableVisitDay = p,
                        AdvertiseId = advertiseId,

                    });
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }
        public async Task<ServiceResult> UpdateAdvertiseAvailableVisitDays(List<DateTimeOffset> SelectedDays, int advertiseId, int userId)
        {
            try
            {
                ValidateModel(advertiseId);

                if (!CheckUserHasThisAdvertise(advertiseId, userId))
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.WrongAdvertise, null);///

                _repoAv.Table.Where(d => d.AdvertiseId == advertiseId)
                    .ToList()
                    .ForEach(d => _repoAv.Delete(d));

                await CreateAdvertiseAvailableVisitDays(SelectedDays, advertiseId, userId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }
        public async Task<ServiceResult> GetAdvertiseAvailableVisitDays(int advertiseId, int userId)
        {
            try
            {
                ValidateModel(advertiseId);

                if (!CheckUserHasThisAdvertise(advertiseId, userId))
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.WrongAdvertise, null);///

                var result = _repoAv.TableNoTracking
                    .Where(u => u.AdvertiseId == advertiseId)
                    .ToList();

                if (result is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///


                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }
        public async Task<ServiceResult> AdvertiserGetRequestsForVisit(int advertiseId, int userId)
        {
            try
            {
                ValidateModel(advertiseId);

                if (!CheckUserHasThisAdvertise(advertiseId, userId))
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.WrongAdvertise, null);///

                var result = _repoReq.TableNoTracking
                    .Where(u => u.AdvertiseId == advertiseId && u.UserIdOfUser != userId && !u.IsDelete)
                      .ToList();

                if (result is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }
        public async Task<ServiceResult> AdvertiserConfirmRequestsForVisit(int reqId, int userId)
        {
            try
            {
                ValidateModel(reqId);

                var result = await _repoReq.Entities
                    .Where(u => u.Id == reqId && u.UserIdOfUser != userId)
                    .FirstOrDefaultAsync();

                if (result is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.AdveriseNotFound, null);///

                if (result.IsConfirm == true)
                    result.IsConfirm = false;

                else if (result.IsConfirm == false)
                {
                    result.IsConfirm = true;

                    // Send mail to users who sent requests

                    #region send  email
                    var email = await GetUserEmail(result.UserIdOfUser);
                    var advName = await GetAdvertiseName(result.AdvertiseId);
                    string body = Resource.EmailSubject1 + "**" + advName + "**" + Resource.EmailSubject1_1;

                    await SendMail.SendAsync(email, Resource.ConfirmVisit, body);
                    #endregion
                }
                _repoReq.Update(result);

                var res = new AdvertiseVisitRequestsDto
                {
                    AdvertiseId = result.AdvertiseId,
                    FullNameOfUser = result.FullNameOfUser,
                    AvailableVisitDay = result.AvailableVisitDay,
                    IsConfirm = result.IsConfirm,
                };

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
        public async Task<ServiceResult> UserRequestsForVisit(int userId)
        {
            try
            {
                ValidateModel(userId);

                var result = _repoReq.TableNoTracking
                    .Where(u => u.UserIdOfUser == userId && !u.IsDelete)
                    .ToList();

                if (result is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }
        public async Task<ServiceResult> UserDeleteRequestsForVisit(int reqId, int userId)
        {
            try
            {
                ValidateModel(userId);

                var result = await _repoReq.TableNoTracking
                    .Where(u => u.UserIdOfUser == userId && !u.IsDelete && u.Id == reqId)
                      .FirstOrDefaultAsync();

                if (result is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                result.IsDelete = true;

                _repoReq.Update(result);

                //var res = new AdvertiseVisitRequestsDto
                //{
                //    AdvertiseId = result.AdvertiseId,
                //    FullNameOfUser = result.FullNameOfUser,
                //    DayOfWeek = result.DayOfWeek,
                //    IsConfirm = result.IsConfirm,
                //};

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }

        #endregion

        #region Mostly used methods
        public bool CheckUserHasThisAdvertise(int advertiseId, int userId)
        {
            ValidateModel(advertiseId);
            ValidateModel(userId);

            var result = _repoAd
                       .TableNoTracking
                       .Any(u => u.UserId == userId && !u.IsDelete && u.IsConfirm && u.Id == advertiseId);

            if (result)
                return true;

            else
                return false;
        }
        public async Task<string> GetUserEmail(int userId)
        {

            ValidateModel(userId);

            var result = await _repo.Entities
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (result is null)
                return null;///

            var email = result.Email;

            return email;

        }
        public async Task<string> GetAdvertiseName(int advetiseId)
        {
            ValidateModel(advetiseId);

            var result = await _repoAd.Entities
                .Where(u => u.Id == advetiseId)
                .FirstOrDefaultAsync();

            if (result is null)
                return null;///

            var advName = result.AdvertiseText;

            return advName;
        }
        public bool CheckRoleExistence(int roleId)
        {
            try
            {
                var exist = _repoR.TableNoTracking.Any(r => r.Id == roleId && !r.IsDelete);

                if (exist)
                    return true;

                else
                    return false;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                throw new Exception(Resource.GeneralErrorTryAgain);
            }
        }
        #endregion

    }
}
