using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Net;

using WebApiCourse.WebFramework.Base;
using Entities.Base;
using Microsoft.AspNetCore.Authorization;
using Common.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApiCourse.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    [SwaggerTag("لیست سرویسهای  کاربر")]
    [ApiVersion("1")]
    public class UserController : APIControllerBase
    {
        //private readonly IUserService _user;
        //private readonly ILogger<UserController> _logger;
        //private readonly IJwtService _jwt;
        //private readonly IRepository<User> _Repo;


        /// <summary>
        ///         [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)] ==> ApiResult it should change to desired output
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Test(CancellationToken cancellationToken)
        {
            return Ok();
        }

    }
}
