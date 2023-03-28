using Common.Resources;
using Entities.Base;
using Entities.Common.ViewModels;
using Entities.Models.User.Advertises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAdminService
    {
        public Task<ServiceResult> GetAllAdvertises(int pageId = 1, string advertiseText = "", string homeAddress = "", string orderBy = "date", string saleType = "all", long startprice = 0, long endprice = 0, long startrentprice = 0, long endrentprice = 0);
        public Task<ServiceResult> CreateAdvertise(UserAdvertiseViewModelForAdmin ua, int userId, CancellationToken cancellationToken);
        public Task<ServiceResult> UpdateAdvertise(int advertiseId, UserAdvertiseViewModelForAdmin ua, CancellationToken cancellationToken);
        public Task<ServiceResult> AddRolesToUser(List<int> SelectedRoles, int userid);
        public Task<ServiceResult> EditRolesUser(List<int> SelectedRoles, int userid);
        //public Task<ServiceResult> GetUserRoles(int userid);
        public Task<ServiceResult> GetRoles();
        public Task<ServiceResult> AddRoles(RoleViewModel role);
        public Task<ServiceResult> EditRoles(int roleId, RoleViewModel role);
        public Task<ServiceResult> GetAllUsers(int pageId = 1, string fullName = "", string phoneNumber = "", string email = "");
        public Task<ServiceResult> CreateUser(UserViewModelFromAdmin user);
        public Task<ServiceResult> EditUser(EditUserViewModelFromAdmin user, int userId);
        #region most used methods
        public Task<bool> RoleExist(string name);
        #endregion
    }
}
