using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
namespace ToDoXamarinDemo
{
    public class BlobManager
    {
        

        public static BlobManager Instance { get; } = new BlobManager();
        const string connectionString = "DefaultEndpointsProtocol=https;AccountName=todoblobs;AccountKey=lP8Wu4Yho+na8so+cR5i3AbURwn/mXtwJIrd7ieI9C516XXRUxgVHTaVhP0kBv0SSZgxYUFE9etnGZJfJkkhLg==";

        CloudBlobClient blobClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();

        CloudBlobContainer imageContainer;
        public BlobManager()
        {

            imageContainer = blobClient.GetContainerReference("todoblobscontainer");
        }


        public async Task<List<Uri>> GetAllBlobUrisAsync(){

            var contToken = new BlobContinuationToken();
            var allBlobs = await imageContainer.ListBlobsSegmentedAsync(contToken).ConfigureAwait(false);
            var uris = allBlobs.Results.Select(b => b.Uri).ToList();

            return uris;
        }


        public async Task<string> UploadAsync(byte[] array) {

            var uniqueBlobName = Guid.NewGuid().ToString();
            //uniqueBlobName += Path.GetExtension(localPath);

            var blobRef = imageContainer.GetBlockBlobReference(uniqueBlobName);
            await blobRef.UploadFromByteArrayAsync(array,0,array.Count()).ConfigureAwait(false);
            return uniqueBlobName;
        }


        public async Task<byte[]> GetFileAsync(string name)
        {
            
            var blob = imageContainer.GetBlobReference(name);
            if (await blob.ExistsAsync())
            {
                await blob.FetchAttributesAsync();
                byte[] blobBytes = new byte[blob.Properties.Length];

                await blob.DownloadToByteArrayAsync(blobBytes, 0);
                return blobBytes;
            }
            return null;
        }
    }

   
}
