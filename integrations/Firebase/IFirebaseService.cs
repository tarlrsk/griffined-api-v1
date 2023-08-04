using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.integrations.Firebase
{
    public interface IFirebaseService
    {
        int GetAzureIdWithToken();
        Task ChangePasswordWithUid(string uid, string newPassword);
        Task<String> UploadRegistrationRequestPaymentFile(int requestId, DateTime createdDate, IFormFile file);
        Task DeleteStorageFileByObjectName(string objectName);
        Task<String> GetUrlByObjectName(string objectName);
        Task<Google.Apis.Storage.v1.Data.Object> GetObjectByObjectName(string objectName);
    }
}