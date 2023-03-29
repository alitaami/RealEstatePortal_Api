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
using Entities.Models.User.Roles;

namespace EstateAgentApi.Controllers
{
    /// <summary>
    /// All methods need authorization 
    /// </summary>
    [SwaggerTag("سرویس های صفحه اصلی")]
    [Authorize(Roles = "3")]
    public class AdminController : APIControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private IAdvertiseService _ad;
        private IRepository<User> _repo;
        IAdminService _admin;

        public AdminController(ILogger<AdminController> logger, IRepository<User> repo, IAdvertiseService advertise, IAdminService admin)
        {
            _logger = logger;
            _ad = advertise;
            _repo = repo;
            _admin = admin;
        }
     
        /// <summary>
        /// Get all advertises
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
        [SwaggerOperation("لیست اگهی ها")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserAdvertisesForHomePage), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAdvertises(int pageId = 1, string advertiseText = "", string homeAddress = "", string orderBy = "date", string saleType = "all", long startprice = 0, long endprice = 0, long startrentprice = 0, long endrentprice = 0)
        {
            var result = await _admin.GetAllAdvertises(pageId, advertiseText, homeAddress, orderBy, saleType, startprice, endprice, startrentprice, endrentprice);

            return APIResponse(result);
        }

        /// <summary>
        /// Create new advertise
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
        public virtual async Task<IActionResult> CreateAdvertise(UserAdvertiseViewModelForAdmin ad, CancellationToken cancellationToken)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _admin.CreateAdvertise(ad, userId.ToInt(), cancellationToken);

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
        [ProducesResponseType(typeof(UserRoles), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateAdvertise(int advertiseId, UserAdvertiseViewModelForAdmin ua, CancellationToken cancellationToken)
        {
            var result = await _admin.UpdateAdvertise(advertiseId, ua, cancellationToken);

            return APIResponse(result);
        }

        /// <summary>
        /// Add roles to users
        /// </summary>
        /// <param name="SelectedRoles"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("نسبت دادن نقش یوزر")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserRoles), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> AddRolesToUser(List<int> SelectedRoles, int userid)
        {

            var result = await _admin.AddRolesToUser(SelectedRoles, userid);

            return APIResponse(result);
        }

        /// <summary>
        /// Edit roles of users
        /// </summary>
        /// <param name="SelectedRoles"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerOperation("آپدیت نقش کاربر")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserRoles), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> EditRolesToUser(List<int> SelectedRoles, int userid)
        {
            var result = await _admin.EditRolesUser(SelectedRoles, userid);

            return APIResponse(result);
        }
        /// <summary>
        /// Get roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("لیست نقش ها")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Role), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
         public async Task<IActionResult> GetRoles()
        {
            var result = await _admin.GetRoles();

            return APIResponse(result);
        }
        /// <summary>
        /// Create role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("ایجاد نقش")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Role), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> AddRoles(RoleViewModel role)
        {
            var result = await _admin.AddRoles(role);

            return APIResponse(result);
        }

        /// <summary>
        /// Update role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerOperation("آپدیت نقش ")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Role), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> EditRoles(int roleId, RoleViewModel role)
        {
            var result = await _admin.EditRoles(roleId, role);

            return APIResponse(result);
        }
        /// <summary>
        /// Get users
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="fullName"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("لیست کاربران ")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(EstateAgentDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
         public async Task<IActionResult> GetAllUsers(int pageId = 1, string fullName = "", string phoneNumber = "", string email = "")
        {
            var result = await _admin.GetAllUsers(pageId,fullName,phoneNumber,email);

            return APIResponse(result);
        }
        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("ایجاد کاربر")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserViewModelFromAdmin), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> CreateUser(UserViewModelFromAdmin user)
        {
            var result = await _admin.CreateUser(user);

            return APIResponse(result);
        }
        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerOperation("آپدیت کاربر ")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(EditUserViewModelFromAdmin), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> EditUser(EditUserViewModelFromAdmin user,int userId)
        {
            var result = await _admin.EditUser(user, userId);

            return APIResponse(result);
        }
    
    }
}
