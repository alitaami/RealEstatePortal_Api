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
using Services.Interfaces;
using Entities.Common.Dtos;
using Entities.Common.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using Entities.Models.Advertises;
using DocumentFormat.OpenXml.Spreadsheet;

namespace WebApiCourse.Controllers.v1
{
    /// <summary>
    /// All methods need authorization 
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
            _user = user;
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
        [Authorize(Roles = "2")]
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
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateEsteAgentInfo(EstateAgentPanelViewModel ua, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.UpdateEstateAgentInfo(userId.ToInt(), ua, cancellationToken);

            return APIResponse(result);
        }

        /// <summary>
        ///  Creating advertises by users
        /// </summary>
        /// <param name="ad"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("ایجاد آگهی")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UserAdvertiseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> CreateAdvertise([FromForm] UserAdvertiseViewModel ad, CancellationToken cancellationToken)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.CreateAdvertise(ad, userId.ToInt(), cancellationToken);

            return APIResponse(result);
        }

        /// <summary>
        /// Get advertises of authenticated user
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="advertiseText"></param>
        /// <param name="homeAddress"></param>
        /// <param name="orderBy"></param>
        /// <param name="saleType"></param>
        /// <param name="startprice"></param>
        /// <param name="endprice"></param>
        /// <param name="startrentprice"></param>
        /// <param name="endrentprice"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("لیست اگهی های کاربر")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserAdvertiseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> GetAdvertisesOfUser(int pageId = 1, string advertiseText = "", string homeAddress = "", string orderBy = "date", string saleType = "all", long startprice = 0, long endprice = 0, long startrentprice = 0, long endrentprice = 0)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.GetAllAdvertisesOfUser(pageId, advertiseText, homeAddress, orderBy, saleType, startprice, endprice, startrentprice, endrentprice, userId.ToInt());

            return APIResponse(result);
        }

        /// <summary>
        /// Show advertise images of user
        /// </summary>
        /// <param name="advertiseId"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("لیست عکس های اگهی")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserAdvertiseDto.AdvertiseImagesDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAdvertiseImagesOfUser(int advertiseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.GetAdvertiseImagesOfUser(advertiseId, userId.ToInt());

            return APIResponse(result);
        }

        /// <summary>
        /// Update advertise of authenticated user
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
        public async Task<IActionResult> UpdateAdvertiseOfUser(int advertiseId, UserUpdateAdvertiseViewModel ua, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.UpdateAdvertiseOfUser(advertiseId, userId.ToInt(), ua, cancellationToken);

            return APIResponse(result);
        }


        /// <summary>
        /// Update each photo of advertise
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="image"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerOperation("آپدیت عکس آگهی")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UserAdvertiseDto.AdvertiseImagesDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateAdvertiseImageOfUser(int fileId, [FromForm] AdvertiseImageViewModel image, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.UpdateAdvertiseImageOfUser(fileId, userId.ToInt(), image, cancellationToken);

            return APIResponse(result);
        }
        /// <summary>
        /// Delete each photo of advertise
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        [SwaggerOperation("حذف عکس آگهی")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UserAdvertiseDto.AdvertiseImagesDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAdvertiseImageOfUser(int fileId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.DeleteAdvertiseImageOfUser(fileId, userId.ToInt(), cancellationToken);

            return APIResponse(result);
        }
        /// <summary>
        /// Delete advertise of authenticated user (** IsDelete == True **)
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

        /// <summary>
        /// Get available visit days for an advertise
        /// </summary>
        /// <param name="advertiseId"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("دریافت روزهای بازدید حضوری")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> GetAdvertiseAvailableVisitDays(int advertiseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.GetAdvertiseAvailableVisitDays(advertiseId, userId.ToInt());

            return APIResponse(result);
        }
        /// <summary>
        /// Add available visit days for an advertise
        /// </summary>
        /// <param name="SelectedDays"></param>
        /// <param name="advertiseId"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("افزودن روزهای بازدید حضوری")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> CreateAdvertiseAvailableVisitDays(List<DateTimeOffset> SelectedDays, int advertiseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.CreateAdvertiseAvailableVisitDays(SelectedDays, advertiseId, userId.ToInt());

            return APIResponse(result);
        }

        /// <summary>
        ///  Edit available visit days for an advertise
        /// </summary>
        /// <param name="SelectedDays"></param>
        /// <param name="advertiseId"></param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerOperation("ویرایش روزهای بازدید حضوری")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateAdvertiseAvailableVisitDays(List<DateTimeOffset> SelectedDays, int advertiseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.UpdateAdvertiseAvailableVisitDays(SelectedDays, advertiseId, userId.ToInt());

            return APIResponse(result);
        }
        /// <summary>
        /// Get all requests of advertises by advertiser
        /// </summary>
        /// <param name="advertiseId"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("دریافت درخواست های بازدید حضوری")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(AdvertiseVisitRequests), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> GetRequestsForVisitByAdvertiser(int advertiseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.AdvertiserGetRequestsForVisit(advertiseId, userId.ToInt());

            return APIResponse(result);
        }
        /// <summary>
        /// Accept or Reject requests of advertises by advertiser
        /// </summary>
        /// <param name="reqId"></param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerOperation("تایید یا عدم تایید درخواست ها")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(AdvertiseVisitRequests), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> ConfirmOrRejectRequestsForVisitByAdvertiser(int reqId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.AdvertiserConfirmRequestsForVisit(reqId, userId.ToInt());

            return APIResponse(result);
        }

        /// <summary>
        /// Get all requests those have been sent by user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("درخواست های ارسال شده")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(AdvertiseVisitRequests), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UserRequestsForVisit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.UserRequestsForVisit(userId.ToInt());

            return APIResponse(result);
        }

        /// <summary>
        /// Delete  requests those have been sent by user 
        /// </summary>
        /// <param name="reqId"></param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerOperation("حذف درخواست ارسال شده")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(AdvertiseVisitRequests), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> UserDeleteRequestsForVisit(int reqId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _user.UserDeleteRequestsForVisit(reqId, userId.ToInt());

            return APIResponse(result);
        }
    }
}
