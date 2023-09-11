using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICountOnlineUsersService
    {
        Task MarkUserAsOnline(string userId);
        Task MarkUserAsOffline(string userId);
        Task<int> CountOnlineUsers();
    }
}
