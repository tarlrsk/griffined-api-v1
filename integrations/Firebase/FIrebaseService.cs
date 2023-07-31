using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Google.Cloud.Storage.V1;

namespace griffined_api.integrations
{
    public class FirebaseService : IFirebaseService
    {
        private string? FIREBASE_BUCKET = Environment.GetEnvironmentVariable("FIREBASE_BUCKET");
        private readonly StorageClient _storageClient;
        private readonly UrlSigner _urlSigner;
        public FirebaseService(StorageClient storageClient, UrlSigner urlSigner)
        {
            _storageClient = storageClient;
            _urlSigner = urlSigner;
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

        public async Task<String> UploadRegistrationRequestPaymentFile(int requestId, DateTime createdDate, IFormFile file)
        {
            var fileName = file.FileName;
            var year = createdDate.Year.ToString();
            var month = createdDate.ToString("MMMM").ToLower();
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
        public async Task DeleteStorageFile(string objectName)
        {
            var storageObject = await _storageClient.GetObjectAsync(FIREBASE_BUCKET, objectName);

            if (storageObject != null)
                await _storageClient.DeleteObjectAsync(storageObject);
            else
                throw new InternalServerException($"Cannot Retrieve Object From {objectName}");
        }
    }
}