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
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.Services
{
    public class UserService : ServiceBase<UserService>, IUserService
    {
        private IRepository<User> _repo;
        private IRepository<UserRoles> _repoUR;
        private IRepository<Role> _repoR;
        private readonly IJwtService _jwtService;
        public UserService(ILogger<UserService> logger, IRepository<User> repository, IJwtService jwtService, IRepository<UserRoles> repoUR, IRepository<Role> repoR) : base(logger)
        {
            _repo = repository;
            _repoUR = repoUR;
            _repoR = repoR;
            _jwtService = jwtService;
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
    }
}
