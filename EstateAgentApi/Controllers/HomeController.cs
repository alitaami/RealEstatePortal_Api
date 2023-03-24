using Entities.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using System.Net;
using WebApiCourse.WebFramework.Base;
using Entities.Models.User;
using Entities.Common.ViewModels;
using Entities.Common.Dtos;

namespace EstateAgentApi.Controllers
{
    [SwaggerTag("سرویس های صفحه اصلی")]
    public class HomeController : APIControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private IAdvertiseService _Ad;

        public HomeController(ILogger<HomeController> logger, IAdvertiseService ad)
        {
            _logger = logger;
            _Ad = ad;

        }
        /// <summary>
        /// Get advertise by searching and pagination
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="advertiseText"></param>
        /// <param name="homeAddress"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("لیست اگهی ها")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserAdvertisesForHomePage), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAdvertises(int pageId = 1, string advertiseText = "", string homeAddress = "")
        {
            var result = await _Ad.GetAllAdvertises(pageId, advertiseText, homeAddress);

            return APIResponse(result);
        }
        
        /// <summary>
        /// Get advertise detail by advertiseId
        /// </summary>
        /// <param name="advertiseId"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("آگهی")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserAdvertiseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> AdvertiseDetail(int advertiseId)
        {
            var result = await _Ad.GetAdveriseForShow(advertiseId);

            return APIResponse(result);
        }

    }
}
