using Entities.Base;
using Entities.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAdvertiseService
    {
        public Task<ServiceResult> GetAllAdvertises(int pageId = 1, string advertiseText = "", string homeAddress = ""  );
    }
}
