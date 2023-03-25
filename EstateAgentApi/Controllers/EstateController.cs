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

    }
}
