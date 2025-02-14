﻿using Entities.Base;
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
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using Common.Utilities;
using DocumentFormat.OpenXml.Spreadsheet;
using Entities.Models.Advertises;
using System;
using Microsoft.AspNetCore.RateLimiting;

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
        [AllowAnonymous]
        [EnableRateLimiting("test")]
        public async Task<IActionResult> GetAdvertises(int pageId = 1, string advertiseText = "", string homeAddress = "", string orderBy = "date", string saleType = "all", long startprice = 0, long endprice = 0, long startrentprice = 0, long endrentprice = 0)
        {
            var result = await _Ad.GetAllAdvertises(pageId, advertiseText, homeAddress, orderBy, saleType, startprice, endprice, startrentprice, endrentprice);

            return APIResponse(result);
        }

        /// <summary>
        /// Get all advertises using elastic search
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="searchTerm"></param>
        /// <param name="homeAddress"></param>
        /// <param name="orderBy"></param>
        /// <param name="saleType"></param>
        /// <param name="startprice"></param>
        /// <param name="endprice"></param>
        /// <param name="startrentprice"></param>
        /// <param name="endrentprice"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("لیست اگهی ها - با استفاده از الاستیک سرچ")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserAdvertisesForHomePage), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        [EnableRateLimiting("test")]
        public async Task<IActionResult> GetAdvertises_ElasticSearch(string searchTerm = "")
        {
            var result = await _Ad.SearchAdvertises_ElasticSearch(searchTerm);

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
        [EnableRateLimiting("test")]
        public async Task<IActionResult> AdvertiseDetail(int advertiseId)
        {
            var result = await _Ad.GetAdveriseForShow(advertiseId);

            return APIResponse(result);
        }
        /// <summary>
        /// Show images of advertise
        /// </summary>
        /// <param name="advertiseId"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("لیست عکس های اگهی")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserAdvertiseDto.AdvertiseImagesDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAdvertiseImages(int advertiseId)
        {
            var result = await _Ad.GetAdvertiseImages(advertiseId);

            return APIResponse(result);
        }
        /// <summary>
        /// Get available visit days of an advertise
        /// </summary>
        /// <param name="advertiseId"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("روزهای بازدید از ملک")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(AdvertiseAvailableVisitDays), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAdvertiseAvailableVisitDays(int advertiseId)
        {
            var result = await _Ad.GetAdvertiseAvailableVisitDays(advertiseId);

            return APIResponse(result);
        }
        /// <summary>
        /// Send request to visit advertise (need authorization)
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <param name="advertiseId"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("ثبت درخواست بازدید")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(AdvertiseVisitRequests), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        [Authorize]
        public async Task<IActionResult> RequestForAdvertiseVisit(DateTimeOffset dayOfWeek, int advertiseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fullname = User.FindFirstValue(ClaimTypes.Name);


            var result = await _Ad.RequestForAdvertiseVisit(dayOfWeek, advertiseId, userId.ToInt(), fullname);

            return APIResponse(result);
        }

    }
}
