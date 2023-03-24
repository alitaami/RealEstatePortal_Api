using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Net;

using WebApiCourse.WebFramework.Base;
using Entities.Base;
using Microsoft.AspNetCore.Authorization;
using Common.Utilities;
using Swashbuckle.AspNetCore.Annotations;
using Entities.Models.User;
using EstateAgentApi.Controllers;
using Services.Interfaces;
using Entities.Common.Dtos;
using Entities.Common.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace WebApiCourse.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    [SwaggerTag("لیست سرویسهای  کاربر")]
    [Authorize(Roles = "2")]
    public class UserController : APIControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private IUserService _user;
        private IRepository<User> _repo;

        public UserController(ILogger<UserController> logger, IRepository<User> repo, IUserService user)
        {
            _logger = logger;
            _user =   user;
            _repo = repo;
        }

        /// <summary>
        /// Get user info for user panel
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("اطلاعات کاربر ")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(EstateAgentDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> GetUserInfo(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.GetUserInfo(userId.ToInt(), cancellationToken);

            return APIResponse(result);
        }

        /// <summary>
        /// Section for update user info in user panel
        /// </summary>
        /// <param name="ua"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerOperation("آپدیت اطلاعات کاربر ")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> UpdateUserInfo(UserPanelViewModel ua, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.UpdateUserInfo(userId.ToInt(), ua, cancellationToken);

            return APIResponse(result);
        }

    }
}
