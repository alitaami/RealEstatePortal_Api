using Common.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Common.Dtos
{
 

        public class EstateAgentDto
        {
            public string UserName { get; set; }

            public string FullName { get; set; }

            public string Email { get; set; }

            public int Age { get; set; }

            public string PhoneNumber { get; set; }

            public string EstateAddress { get; set; }

            public string EstatePhoneNumber { get; set; }

            public string EstateCode { get; set; }
            public DateTimeOffset? LastLoginDate { get; set; }
        }

        public class UserDto
        {
            public string UserName { get; set; }

            public string FullName { get; set; }

            public string Email { get; set; }

            public int Age { get; set; }

            public string PhoneNumber { get; set; }

            public DateTimeOffset? LastLoginDate { get; set; }
     
    }
}
