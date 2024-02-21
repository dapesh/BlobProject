using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Net;
using Microsoft.Azure;
using Azure.Storage.Sas;
using Azure.Storage;

namespace BlobProject
{
    public class ImageService
    {
        private readonly IConfiguration _configuration;
        static string _baseUrl = string.Empty;
        static string account = string.Empty;
        static string key = string.Empty;
        public ImageService(IConfiguration configuration)
        {
            _configuration = configuration;
            _baseUrl = Environment.GetEnvironmentVariable("StorageUrl") ?? _configuration["StorageUrl"].ToString();
            account = Environment.GetEnvironmentVariable("StorageAccountName") ?? _configuration["StorageAccountName"].ToString();
            key = Environment.GetEnvironmentVariable("StorageAccountKey") ?? _configuration["StorageAccountKey"].ToString();
        }
        //static string account = Environment.GetEnvironmentVariable("StorageAccountName") ?? CloudConfigurationManager.GetSetting("StorageAccountName");
        //static string key = Environment.GetEnvironmentVariable("StorageAccountKey") ?? CloudConfigurationManager.GetSetting("StorageAccountKey");
        public string UploadImageToBlob(byte[] bytes, string filename = null, string flag = null)
        {
            string imageFullPath = string.Empty;
            string strpath = System.IO.Path.GetExtension(filename);
            try
            {
                Stream stream = new MemoryStream(bytes);
                CloudStorageAccount cloudStorageAccount = ConnectionString.GetConnectionString(account, key);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer;
                if (flag == "p")
                {
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("partnerdocuments");

                }
                else if (flag == "t")
                {
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("transactiondocuments");

                }
                else
                {
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("customerdocuments");

                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                bool isCreated = cloudBlobContainer.CreateIfNotExistsAsync().Result;
                if (isCreated) cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Off });

                string imageName = Guid.NewGuid().ToString();

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);
                if (strpath == ".pdf")
                {
                    cloudBlockBlob.Properties.ContentType = "application/pdf";
                }
                else
                {
                    cloudBlockBlob.Properties.ContentType = "image/jpeg";
                }
                cloudBlockBlob.UploadFromStreamAsync(stream);
                imageFullPath = cloudBlockBlob.Uri.ToString();
            }
            catch
            {

            }
            return imageFullPath;

        }
        public static string GetBlobImage(string containername, string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }
            var blobSasBuilder = new AccountSasBuilder()
            {
                Services = Azure.Storage.Sas.AccountSasServices.All,
                ResourceTypes = Azure.Storage.Sas.AccountSasResourceTypes.All,
                StartsOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddHours(1),//Let SAS token expire after 5 minutes.
            };
            blobSasBuilder.SetPermissions(Azure.Storage.Sas.AccountSasPermissions.Read);//User will only be able to read the blob and it's properties
            var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(account, key)).ToString();
            //var baseUrl = Environment.GetEnvironmentVariable("StorageUrl") ?? CloudConfigurationManager.GetSetting("StorageUrl");
            var preUrl = _baseUrl + containername + "/" + filename;
            var imgUrl = $"{preUrl}?{sasToken}";
            return imgUrl;
        }

    }
}
