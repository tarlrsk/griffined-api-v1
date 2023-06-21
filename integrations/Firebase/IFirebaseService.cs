using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.integrations.Firebase
{
    public interface IFirebaseService
    {
        Task ChangePasswordWithUid(string uid, string newPassword);
    }
}