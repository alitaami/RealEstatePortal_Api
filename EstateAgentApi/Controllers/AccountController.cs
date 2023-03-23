﻿using Entities.Base;
using Entities.Common.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApiCourse.WebFramework.Base;
using Services.Interfaces;
using Entities.Models.User;
using System.Threading;
using Newtonsoft.Json.Linq;
using Entities.ViewModels;
using Common.Resources;
using Common.Utilities;
using Services.Interfaces.Services;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EstateAgentApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [SwaggerTag("سرویس های احراز هویت")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : APIControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private IAccountService _acc;
        private IJwtService _jwt;
        private IRepository<User> _repo;


        public AccountController(ILogger<AccountController> logger, IAccountService acc, IJwtService jwt, IRepository<User> repo)
        {
            _logger = logger;
            _acc = acc;
            _jwt = jwt;
            _repo = repo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("ثبت نام کاربر")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> UserSignUp(UserViewModel user, CancellationToken cancellationToken)
        {
            var result = await _acc.UserSignUp(user, cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation("ثبت نام املاک")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> EstateUserSignUp(EstateUserViewModel user, CancellationToken cancellationToken)
        {
            var result = await _acc.EstateUserSignUp(user, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("ورود")]
        [ProducesResponseType(typeof(JwtToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] TokenRequest tokenRequest, CancellationToken cancellationToken)
        {
            var result = await _acc.Login(tokenRequest, cancellationToken);

            return APIResponse(result);

        }
    }
}
