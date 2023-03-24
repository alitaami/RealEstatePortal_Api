using Entities.Base;
using Entities.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        public Task<ServiceResult> GetUserInfo(int userId, CancellationToken cancellationToken);
        public Task<ServiceResult> UpdateUserInfo(int userId, UserPanelViewModel user, CancellationToken cancellationToken);

    }
}
