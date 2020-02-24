using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ddacAPI.Util
{
    public class BlobManager
    {
        public static IConfiguration Configuration { get; set; }
        public static CloudBlobContainer Container { get; set; }
        public static CloudBlobClient BlobClient { get; set; }
        public static CloudStorageAccount StorageAccount { get; set; }
        public BlobManager(IConfiguration configuration)
        {
            //Get connection string from configuration
            Configuration = configuration;
        }
        public async Task CreateCloudBlobContainerAsync()
        {
            if (Container == null)
            {
                StorageAccount = CloudStorageAccount.Parse(
                    Environment.GetEnvironmentVariable("AZURE_BLOB_STORAGE_CONNECTION_STRING") ??
                    Configuration.GetConnectionString("AZURE_BLOB_STORAGE_CONNECTION_STRING")
                );
                BlobClient = StorageAccount.CreateCloudBlobClient();
                Container = BlobClient.GetContainerReference(
                    Environment.GetEnvironmentVariable("AZURE_BLOB_STORAGE_CONTAINER_REFERENCE") ??
                    Configuration.GetConnectionString("AZURE_BLOB_STORAGE_CONTAINER_REFERENCE")
                );
                await Container.CreateIfNotExistsAsync();
            }
        }
        public async Task<string> UploadFileToStorageAsync(Stream fileStream, string fileName)
        {
            try
            {
                if (Container == null)
                {
                    await CreateCloudBlobContainerAsync();
                }
                CloudBlockBlob blockBlob = Container.GetBlockBlobReference(fileName);
                await blockBlob.UploadFromStreamAsync(fileStream);
                return blockBlob.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        public async Task<bool> DeleteFileFromStorageAsync(string fileName)
        {
            try
            {
                if (Container == null)
                {
                    await CreateCloudBlobContainerAsync();
                }
                CloudBlockBlob blockBlob = Container.GetBlockBlobReference(fileName);
                await blockBlob.DeleteIfExistsAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}
