namespace griffined_api.Integrations.Firebase
{
    public interface IFirebaseService
    {
        int GetAzureIdWithToken();
        string GetRoleWithToken();
        Task ChangePasswordWithUid(string uid, string newPassword);
        Task<String> UploadRegistrationRequestPaymentFile(int requestId, DateTime createdDate, IFormFile file);
        Task DeleteStorageFileByObjectName(string objectName);
        Task<String> GetUrlByObjectName(string objectName);
        Task<String> GetContentTypeByObjectName(string objectName);
        Task<Google.Apis.Storage.v1.Data.Object> GetObjectByObjectName(string objectName);
    }
}