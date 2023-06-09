using Azure.Identity;
using Azure.Storage.Blobs;
using suopCommerce.Models;
using System.ComponentModel;

namespace SuopCommerce.Utils
{
    public static class FileUpload
    {
        public const string baseBlobUrl = "https://suopstorageone.blob.core.windows.net";

        public static async Task<List<string>> ImageAsync(IFormFileCollection files)
        {
        StoreContext db = new();
        string containerName = "images";
            List<string> blobs = new();
            //connect to azure
            var blobServiceClient = new BlobServiceClient(
                new Uri(baseBlobUrl),
                new DefaultAzureCredential());
            if(blobServiceClient is null)
            {
                throw new ArgumentNullException(nameof(blobServiceClient));
            }


            
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);


            foreach(var file in files)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if(!validateExtension(extension))
                {
                    break;
                }
                string blobName = Uuid.GetUuid() + extension;
                string blobUrl = $"{baseBlobUrl}/{containerName}/{blobName}";
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    
                    await blobClient.UploadAsync(ms);
                    blobs.Add(blobUrl);
                }
                Image tempImage = new();
                tempImage.Url = blobUrl;
                db.Images.Add(tempImage);
                db.SaveChanges();
            }
            return blobs;
        }


        private static bool validateExtension(string extension) {
            string[] permittedExtensions = { ".png", ".jpg", ".jpeg", ".webp" };
            if (permittedExtensions.Contains(extension))
            {
                return true;
            } else
            {
                return false;
            }
    }
}
}
