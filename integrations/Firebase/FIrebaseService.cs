using FirebaseAdmin.Auth;
using Google.Cloud.Storage.V1;

namespace griffined_api.Integrations.Firebase
{
    public class FirebaseService : IFirebaseService
    {
        private string? FIREBASE_BUCKET = Environment.GetEnvironmentVariable("FIREBASE_BUCKET");
        private readonly StorageClient _storageClient;
        private readonly UrlSigner _urlSigner;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FirebaseService(StorageClient storageClient, UrlSigner urlSigner, IHttpContextAccessor httpContextAccessor)
        {
            _storageClient = storageClient;
            _urlSigner = urlSigner;
            _httpContextAccessor = httpContextAccessor;
        }
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

        public int GetAzureIdWithToken()
        {
            var azureId = Int32.Parse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("azure_id") ?? "0");
            return azureId;
        }

        public string GetRoleWithToken()
        {
            var role = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Role) ?? "";
            return role;
        }

        // Google Storage
        public async Task<string> UploadRegistrationRequestPaymentFile(int requestId, DateTime createdDate, IFormFile file)
        {
            var fileName = file.FileName;
            var year = createdDate.Year.ToString();
            var month = $"{createdDate.Month.ToString()}-{createdDate.ToString("MMMM").ToLower()}";
            var objectName = $"registration-requests/{year}/{month}/{requestId}/{fileName}";
            using (var stream = file.OpenReadStream())
            {
                var storageObject = await _storageClient.UploadObjectAsync(
                    FIREBASE_BUCKET,
                    objectName,
                    file.ContentType,
                    stream
                );
            }
            return objectName;
        }
        public async Task DeleteStorageFileByObjectName(string objectName)
        {
            var storageObject = await _storageClient.GetObjectAsync(FIREBASE_BUCKET, objectName);

            if (storageObject != null)
                await _storageClient.DeleteObjectAsync(storageObject);
            else
                throw new InternalServerException($"Cannot Retrieve Object From {objectName}");
        }

        public async Task<string> GetUrlByObjectName(string objectName)
        {
            string url = await _urlSigner.SignAsync(FIREBASE_BUCKET, objectName, TimeSpan.FromHours(1));
            return url;
        }

        public async Task<string> GetContentTypeByObjectName(string objectName)
        {
            var objectMetaData = await _storageClient.GetObjectAsync(FIREBASE_BUCKET, objectName);
            if (objectMetaData == null)
                throw new InternalServerException("ObjectMetaData is not found.");

            var contentType = objectMetaData.ContentType;
            return contentType;
        }

        public async Task<Google.Apis.Storage.v1.Data.Object> GetObjectByObjectName(string objectName)
        {
            var objectMetaData = await _storageClient.GetObjectAsync(FIREBASE_BUCKET, objectName);
            if (objectMetaData == null)
                throw new InternalServerException("ObjectMetaData is not found");
            return objectMetaData;
        }
    }
}