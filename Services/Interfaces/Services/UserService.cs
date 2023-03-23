using Data;
using Data.Repositories;
using Entities.Models.User;
using EstateAgentApi.Services.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.Services
{
    public class UserService : ServiceBase<UserService>, IUserService
    {
        private IRepository<User> _repo;
        private IRepository<UserRoles> _repoUR;
        private IRepository<Role> _repoR;
        private readonly IJwtService _jwtService;
        public UserService(ILogger<UserService> logger, IRepository<User> repository, IJwtService jwtService, IRepository<UserRoles> repoUR, IRepository<Role> repoR) : base(logger)
        {
            _repo = repository;
            _repoUR = repoUR;
            _repoR = repoR;
            _jwtService = jwtService;
        }
    }
}
