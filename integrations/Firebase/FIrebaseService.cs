using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;

namespace griffined_api.integrations
{
    public class FirebaseService : IFirebaseService
    {
        public async Task ChangePasswordWithUid(string uid, string newPassword)
        {
            UserRecordArgs args = new UserRecordArgs()
            {
                Uid = uid,
                Password = newPassword
            };
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
            var response = new ServiceResponse<ChangeUserPasswordDto>();
        }
    }
}