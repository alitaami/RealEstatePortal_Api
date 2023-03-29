using Entities.Base;
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
using Entities.Common.Dtos;

namespace EstateAgentApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [SwaggerTag("سرویس های احراز هویت")]
  
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
        /// SignUp for Users
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


        /// <summary>
        /// SignUp for Estates 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("ثبت نام املاک")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> EstateAgentSignUp(EstateUserViewModel user, CancellationToken cancellationToken)
        {
          
            var result = await _acc.EstateAgentSignUp(user, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// login with username and password
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
        /// <summary>
        /// Activate Account
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("فعال سازی حساب کاربری")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> ActivateUser(Guid guid, CancellationToken cancellationToken)
        {
            var result = await _acc.ActivateUser(guid, cancellationToken);

            return APIResponse(result);

        }
        /// <summary>
        /// another Activate section,if you wanna activate after a long time 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation(" فعال سازی حساب کاربری مدت ها بعد از ثبت نام")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> SendActivationAfterLongtime(string username, CancellationToken cancellationToken)
        {
            var result = await _acc.SendActivationAfterLongtime(username, cancellationToken);

            return APIResponse(result);

        }
        /// <summary>
        /// Forget password section
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("ارسال کد بازیابی")]
        [ProducesResponseType(typeof(UserForgetPassword), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, CancellationToken cancellationToken)
        {
            var result = await _acc.ForgotPassword(model, cancellationToken);

            return APIResponse(result);

        }

        /// <summary>
        /// Checking the recovery code after get code from email
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("بررسی کدبازیابی")]
        [ProducesResponseType(typeof(UserForgetPassword), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> RecoveryKeyCheck(RecoveryCodeViewModel model, CancellationToken cancellationToken)
        {
            var result = await _acc.RecoveryKey(model, cancellationToken);

            return APIResponse(result);

        }
        /// <summary>
        /// Assign new password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("بازیابی رمزعبور")]
        [ProducesResponseType(typeof(UserForgetPassword), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> AssignNewPassword(int userId, AssignNewPasswordViewModel model, CancellationToken cancellationToken)
        {
            var result = await _acc.AssignNewPassword(userId,model, cancellationToken);

            return APIResponse(result);

        }
   
    }
}
