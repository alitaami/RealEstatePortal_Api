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

namespace EstateAgentApi.Controllers
{
    [SwaggerTag("سرویس های صفحه اصلی")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HomeController : APIControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private IAdvertiseService _Ad;

        public HomeController(ILogger<HomeController> logger, IAdvertiseService ad)
        {
            _logger = logger;
            _Ad = ad;

        }

        [HttpGet]
        [SwaggerOperation("لیست اگهی ها")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(List<UserAdvertises>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAdvertises(int pageId = 1, string advertiseText = "", string homeAddress = "")
        {
            var result = await _Ad.GetAllAdvertises(pageId, advertiseText, homeAddress);

            return APIResponse(result);
        }
    }
}
