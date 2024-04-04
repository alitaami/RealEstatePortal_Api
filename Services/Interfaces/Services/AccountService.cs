using Common.Resources;
using Common.Utilities;
using Data.Repositories;
using DocumentFormat.OpenXml.Spreadsheet;
using Entities.Base;
using Entities.Common.ViewModels;
using Entities.Models.Roles;
using Entities.Models.User;
using Entities.ViewModels;
using EstateAgentApi.Services.Base;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Services.Interfaces.Services
{
    public class AccountService : ServiceBase<AccountService>, IAccountService
    {
        private IRepository<User> _repo;
        private IRepository<UserForgetPassword> _repof;
        private IRepository<UserRoles> _repoUR;
        private IRepository<Role> _repoR;
        private IJwtService _jwtService;
        private readonly IUserService _user;
        private readonly ICountOnlineUsersService _onlineUsersService;

        public AccountService(ICountOnlineUsersService onlineUsersService, IUserService user,ILogger<AccountService> logger, IJwtService jwtService, IRepository<UserForgetPassword> repof, IRepository<User> repo, IRepository<UserRoles> repoUR, IRepository<Role> repoR) : base(logger)
        {
            _onlineUsersService = onlineUsersService;
            _user = user;
            _repo = repo;
            _repoUR = repoUR;
            _repoR = repoR;
            _jwtService = jwtService;
            _repof = repof;
        }

        public async Task<ServiceResult> UserSignUp(UserViewModel user, CancellationToken cancellationToken)
        {
            try
            {
                ValidateModel(user);

                var checkDb = _repo.TableNoTracking
                    .Where(u => u.UserName == user.UserName || u.Email == user.Email || u.PhoneNumber == user.PhoneNumber);

                if (checkDb.Any())
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.AlreadyExists, null);///

                var PasswordHash = SecurityHelper.GetSha256Hash(user.Password);

                var u = new User
                {
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    PasswordHash = PasswordHash,
                    FullName = user.FullName,
                    Age = user.Age,
                    Email = user.Email,
                    IsActive = false,
                    LastLoginDate = DateTimeOffset.Now,

                };

                await _repo.AddAsync(u, cancellationToken);

                if (await CheckRoleExistence(2))
                {
                    var userRole = new UserRoles
                    {
                        RoleId = 2,
                        UserId = u.Id
                    };
                    _repoUR.Add(userRole);
                }

                else
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.RoleNotFound, null);///


                #region send email
                var email = u.Email;
                string body = Resource.ActivateAccount + "<p>" + "https://localhost:7076/api/Account/ActivateUser?guid=" + u.ActivationGuid + "</p>";

                await SendMail.SendAsync(email, Resource.ActivateAccountSubject, body);
                #endregion

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }
        public async Task<ServiceResult> EstateAgentSignUp(EstateUserViewModel user, CancellationToken cancellationToken)
        {
            try
            {
                ValidateModel(user);

                var checkDb = _repo.TableNoTracking
                   .Where(u => u.UserName == user.UserName || u.Email == user.Email || u.PhoneNumber == user.PhoneNumber || u.EstateCode == user.EstateCode);

                if (checkDb.Any())
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.AlreadyExists2, null);///

                var PasswordHash = SecurityHelper.GetSha256Hash(user.Password);

                var u = new User
                {
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    PasswordHash = PasswordHash,
                    FullName = user.FullName,
                    EstateCode = user.EstateCode,
                    Age = user.Age,
                    Email = user.Email,
                    IsActive = true,
                    IsEstateConsultant = true,
                    EstateAddress = user.EstateAddress,
                    EstatePhoneNumber = user.EstatePhoneNumber,
                    LastLoginDate = DateTimeOffset.Now,

                };

                await _repo.AddAsync(u, cancellationToken);

                if (await CheckRoleExistence(1))
                {
                    var userRole = new UserRoles
                    {
                        RoleId = 1,
                        UserId = u.Id
                    };
                    _repoUR.Add(userRole);
                }

                else
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.RoleNotFound, null);///

                #region send email
                var email = u.Email;
                string body = Resource.ActivateAccount + "<p>" + "https://localhost:7076/api/Account/ActivateUser?guid=" + u.ActivationGuid + "</p>";

                await SendMail.SendAsync(email, Resource.ActivateAccountSubject, body);
                #endregion

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }
        public async Task<ServiceResult> Login(TokenRequest tokenRequest, CancellationToken cancellationToken)
        {

            try
            {
                if (!tokenRequest.grant_type.Equals("password", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(ErrorCodeEnum.BadRequest, "OAuth flow should be password !!", null);

                // Check user credentials

                var passwordHash = SecurityHelper.GetSha256Hash(tokenRequest.password);

                var result = await _repo.Table
                    .Where(p => p.UserName == tokenRequest.username && p.PasswordHash == passwordHash && p.IsActive)
                    .FirstOrDefaultAsync(cancellationToken);

                if (result is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                if (!result.IsActive)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.UserIsNotActive, null);///

                result.LastLoginDate = DateTimeOffset.Now;
                _repo.Update(result);


                string userId = await  _user.GetUserIdByUsername(tokenRequest.username);
                await _onlineUsersService.MarkUserAsOnline(userId);

                // Generate JWT token

                var token = await _jwtService.Generate(result);

                return Ok(token);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }
        public async Task<ServiceResult> ForgotPassword(ForgotPasswordViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _repo.Table
                    .Where(u => u.UserName == model.Username && u.IsActive)
                    .FirstOrDefaultAsync(cancellationToken);

                if (user is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                var RecoveryCode = Guid.NewGuid().ToString();

                #region send email
                var email = user.Email;
                string body = Resource.ForgotPassword1 + "<p>" + RecoveryCode + "</p>";

                await SendMail.SendAsync(email, Resource.RecoveryPassword, body);
                #endregion

                var uf = new UserForgetPassword
                {
                    UserId = user.Id,
                    RecoveryKey = RecoveryCode,
                    CreatedDate = DateTimeOffset.Now,
                    ExpireDate = DateTimeOffset.Now.AddMinutes(5),

                };

                _repof.Add(uf);

                return Ok(uf);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
        public async Task<ServiceResult> RecoveryKey(RecoveryCodeViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (model.RecoveryCode is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                var userForgot = await _repof.TableNoTracking
                         .Where(f => f.RecoveryKey == model.RecoveryCode)
                          .FirstOrDefaultAsync(cancellationToken);

                if (userForgot == null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///


                if (userForgot.ExpireDate > DateTimeOffset.Now)
                    return Ok(userForgot);

                else
                {
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.ExpiredDate, null);///
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
        public async Task<ServiceResult> AssignNewPassword(int userId, AssignNewPasswordViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _repo.TableNoTracking
                    .Where(u => u.Id == userId && u.IsActive)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///
                }

                var newHashPass = SecurityHelper.GetSha256Hash(model.NewPassword);

                user.PasswordHash = newHashPass;

                _repo.Update(user);

                // delete records of this user in UserForgetPassword because it is useless

                var userForgetPassword = _repof.Entities.Where(u => u.UserId == userId).ToList();

                foreach (var item in userForgetPassword)
                    _repof.Delete(item);

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
        public async Task<ServiceResult> ActivateUser(Guid guid, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _repo.TableNoTracking
                    .Where(u => u.ActivationGuid == guid)
                    .FirstOrDefaultAsync();

                if (user is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                if (user.IsActive)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.IsActive, null);///

                user.IsActive = true;
                user.ActivationGuid = Guid.NewGuid();

                await _repo.UpdateAsync(user, cancellationToken);

                BackgroundJob.Schedule(() => SendMail.SendAsync(user.Email, "املاک آنلاین", $" {user.FullName}عزیز به املاک آنلاین خوش آمدید "), TimeSpan.FromSeconds(5));

                return Ok(user);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
        public async Task<ServiceResult> SendActivationAfterLongtime(string username, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _repo.TableNoTracking
                    .Where(u => u.UserName == username)
                    .FirstOrDefaultAsync();

                if (user is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                if (user.IsActive)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.IsActive, null);///

                #region send email
                var email = user.Email;
                string body = Resource.ActivateAccount + "<p>" + "https://localhost:7076/api/Account/ActivateUser?guid=" + user.ActivationGuid + "</p>";

                await SendMail.SendAsync(email, Resource.ActivateAccountSubject, body);
                #endregion

                return Ok(user);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }

        public async Task<bool> CheckRoleExistence(int roleId)
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

    }
}
