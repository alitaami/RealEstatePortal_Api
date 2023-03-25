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
    [Authorize]
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

        /// <summary>
        /// Section for update Agent info in EstateAgent panel
        /// </summary>
        /// <param name="ua"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        [HttpPut]
        [SwaggerOperation("آپدیت ادمین املاک")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> UpdateEsteAgentInfo(EstateAgentPanelViewModel ua, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.UpdateEstateAgentInfo(userId.ToInt(), ua, cancellationToken);

            return APIResponse(result);
        }


        /// <summary>
        ///  Creating advertises by EstateAgent
        /// </summary>
        /// <param name="ad"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("ایجاد آگهی")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserAdvertiseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> CreateAdvertise(UserAdvertiseViewModel ad, CancellationToken cancellationToken)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.CreateAdvertise(ad, userId.ToInt(), cancellationToken);

            return APIResponse(result);
        }

        /// <summary>
        /// Get advertises of own Estate
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="advertiseText"></param>
        /// <param name="homeAddress"></param>
        /// <returns></returns>

        [HttpGet]
        [SwaggerOperation("لیست اگهی های املاک")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserAdvertiseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> GetAdvertisesOfUser(int pageId = 1, string advertiseText = "", string homeAddress = "", string orderBy = "date", string saleType = "sale", long startprice = 0, long endprice = 0, long startrentprice = 0, long endrentprice = 0)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.GetAllAdvertisesOfUser(pageId, advertiseText, homeAddress,orderBy,saleType ,startprice , endprice, startrentprice , endrentprice , userId.ToInt());

            return APIResponse(result);
        }
        /// <summary>
        /// Update advertise
        /// </summary>
        /// <param name="advertiseId"></param>
        /// <param name="ua"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerOperation("آپدیت آگهی")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserAdvertiseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> UpdateAdvertiseOfUser(int advertiseId, UserAdvertiseViewModel ua, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.UpdateAdvertiseOfUser(advertiseId, userId.ToInt(), ua, cancellationToken);

            return APIResponse(result);
        }

        /// <summary>
        /// Delete advertise (** IsDelete == True **)
        /// </summary>
        /// <param name="advertiseId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerOperation("حذف آگهی")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> DeleteAdvertiseOfUser(int advertiseId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.DeleteAdvertiseOfUser(advertiseId, userId.ToInt(), cancellationToken);

            return APIResponse(result);
        }
 


    }
}
