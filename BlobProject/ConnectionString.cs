using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using System;
namespace BlobProject
{
    public static class ConnectionString
    {
        //static string account = Environment.GetEnvironmentVariable("StorageAccountName") ?? CloudConfigurationManager.GetSetting("StorageAccountName");
        //static string key = Environment.GetEnvironmentVariable("StorageAccountKey") ?? CloudConfigurationManager.GetSetting("StorageAccountKey");
        public static CloudStorageAccount GetConnectionString(string account, string key)
        {
            string connectionString = $@"DefaultEndpointsProtocol=https;AccountName={account};AccountKey={key}";
            return CloudStorageAccount.Parse(connectionString);
        }
    }
}
