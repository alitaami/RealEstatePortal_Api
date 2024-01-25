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
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using Common.Utilities;
using DocumentFormat.OpenXml.Spreadsheet;
using Entities.Models.Advertises;
using System;
using Carter;

namespace EstateAgentApi.MinimalApi
{
    /// <summary>
    /// 
    /// </summary>
    [SwaggerTag("سرویس های صفحه اصلی")]
    public class Home_MinimalApi : /* برای اینکه بتوان از هرجا فراخوانی کرد  بدون تبدیل به استاتیک */ ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/Home/");

            group.MapGet("GetAdvertises", GetAdvertises).WithName(nameof(GetAdvertises)).AllowAnonymous();
            group.MapGet("AdvertiseDetail", AdvertiseDetail).WithName(nameof(AdvertiseDetail)).AllowAnonymous();
            group.MapGet("GetAdvertiseImages", GetAdvertiseImages).WithName(nameof(GetAdvertiseImages)).AllowAnonymous();
            group.MapGet("GetAdvertiseAvailableVisitDays", GetAdvertiseAvailableVisitDays).WithName(nameof(GetAdvertiseAvailableVisitDays)).AllowAnonymous();
            group.MapPost("RequestForAdvertiseVisit", RequestForAdvertiseVisit).WithName(nameof(RequestForAdvertiseVisit)).AllowAnonymous();
        }

        public static async Task<IResult> GetAdvertises
            (
              IAdvertiseService Ad
            , int pageId = 1
            , string advertiseText = ""
            , string homeAddress = ""
            , string orderBy = "date", string saleType = "all"
            , long startprice = 0
            , long endprice = 0
            , long startrentprice = 0
            , long endrentprice = 0
           )
        {
            var result = await Ad.GetAllAdvertises(pageId, advertiseText, homeAddress, orderBy, saleType, startprice, endprice, startrentprice, endrentprice);

            return Results.Ok(result);
        }

        public static async Task<IResult> AdvertiseDetail
           (IAdvertiseService Ad
            , int advertiseId)
        {
            var result = await Ad.GetAdveriseForShow(advertiseId);

            return Results.Ok(result);
        }

        public static async Task<IResult> GetAdvertiseImages
            (IAdvertiseService Ad
            , int advertiseId)
        {
            var result = await Ad.GetAdvertiseImages(advertiseId);

            return Results.Ok(result);
        }

        public static async Task<IResult> GetAdvertiseAvailableVisitDays
           (IAdvertiseService Ad
            , int advertiseId)
        {
            var result = await Ad.GetAdvertiseAvailableVisitDays(advertiseId);

            return Results.Ok(result);
        }
        public static async Task<IResult> RequestForAdvertiseVisit
          (IAdvertiseService Ad
            , IHttpContextAccessor httpContextAccessor
            , DateTimeOffset dayOfWeek
            , int advertiseId)
        {
            var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fullname = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);


            var result = await Ad.RequestForAdvertiseVisit(dayOfWeek, advertiseId, userId.ToInt(), fullname);

            return Results.Ok(result);
        }
    }
}
