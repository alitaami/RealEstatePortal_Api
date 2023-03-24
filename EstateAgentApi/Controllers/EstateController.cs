using Entities.Base;
using Entities.Common.ViewModels;
using Entities.Models.User;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using System.Net;
using Entities.Common.Dtos;
using Data.Repositories;
using Services.Interfaces;
using WebApiCourse.WebFramework.Base;
using Microsoft.AspNetCore.Authorization;
using Common;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Common.Utilities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EstateAgentApi.Controllers
{
    [SwaggerTag("سرویس های املاک")]
    [Authorize(Roles = "1")]

    public class EstateController : APIControllerBase
    {
        private readonly ILogger<EstateController> _logger;
        private IAdvertiseService _ad;
        private IRepository<User> _repo;


        public EstateController(ILogger<EstateController> logger, IRepository<User> repo, IAdvertiseService advertise)
        {
            _logger = logger;
            _ad = advertise;
            _repo = repo;
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

            var result = await _ad.CreateAdvertise(ad, userId.ToInt(), cancellationToken);

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

        public async Task<IActionResult> GetAdvertisesOfAgent(int pageId = 1, string advertiseText = "", string homeAddress = "")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _ad.GetAllAdvertisesOfAgent(pageId, advertiseText, homeAddress, userId.ToInt());

            return APIResponse(result);
        }

        /// <summary>
        /// Get Agent info for EstateAgent panel
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("اطلاعات ادمین املاک")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(EstateAgentDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> GetEstateAgentInfo(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _ad.GetEstateAgentInfo(userId.ToInt(), cancellationToken);

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

            var result = await _ad.UpdateEstateAgentInfo(userId.ToInt(), ua, cancellationToken);

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

        public async Task<IActionResult> UpdateAdvertiseOfAgent(int advertiseId, UserAdvertiseViewModel ua, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _ad.UpdateAdvertiseOfAgent(advertiseId, userId.ToInt(), ua, cancellationToken);

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

        public async Task<IActionResult> DeleteAdvertiseOfAgent(int advertiseId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _ad.DeleteAdvertiseOfAgent(advertiseId, userId.ToInt(), cancellationToken);

            return APIResponse(result);
        }
    }
}
