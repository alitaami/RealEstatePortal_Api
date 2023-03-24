using Azure.Core;
using Entities.Base;
using Entities.Models.User;
using Services;
namespace Services.Interfaces
{
    public interface IJwtService
    {
        Task<AccessToken> Generate(User user);

        //public string GenerateToken(User user);
    }
}