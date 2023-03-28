using Entities.Base;
using Entities.Common.ViewModels;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAccountService
    {
        public Task<ServiceResult> UserSignUp(UserViewModel user, CancellationToken cancellationToken);
        public Task<ServiceResult> EstateAgentSignUp(EstateUserViewModel user, CancellationToken cancellationToken);
        public Task<ServiceResult> Login(TokenRequest tokenRequest, CancellationToken cancellationToken);
        public Task<ServiceResult> ForgotPassword(ForgotPasswordViewModel model, CancellationToken cancellationToken);
        public Task<ServiceResult> RecoveryKey(RecoveryCodeViewModel model, CancellationToken cancellationToken);
        public Task<ServiceResult> AssignNewPassword(int userId, AssignNewPasswordViewModel model, CancellationToken cancellationToken);


    }
}
