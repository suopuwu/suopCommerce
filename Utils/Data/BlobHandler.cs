using Azure.Identity;
using Azure.Storage.Blobs;
using suopCommerce.Models;
using System.ComponentModel;

namespace SuopCommerce.Utils.Data
{
    public static class BlobHandler
    {
        public const string baseBlobUrl = "https://suopstorageone.blob.core.windows.net";
        public static StoreContext db;
        public static BlobServiceClient blobServiceClient;

        static BlobHandler()
        {
            db = new();
            blobServiceClient = new BlobServiceClient(
                new Uri(baseBlobUrl),
                new DefaultAzureCredential());
            if (blobServiceClient is null)
            {
                throw new ArgumentNullException(nameof(blobServiceClient));
            }

        }
        public static async Task<List<string>> UploadImageAsync(IFormFileCollection files)
        {
            string containerName = "images";
            List<string> blobs = new();
            //connect to azure


            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);


            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!validateImageExtension(extension))
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

        public static async Task<string> DeleteImageAsync(string url)
        {
            string containerName = "images";
            string blobName = url.Split('/').Last();

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            if (await containerClient.DeleteBlobIfExistsAsync(blobName))
            {
                Image? imageToDelete = db.Images.Find(url);
                if (imageToDelete == null)
                {
                    return "failed to delete, no such image at url " + url;
                }

                db.Images.Remove(imageToDelete);
                db.SaveChanges();

            }
            return "deleted the image at url " + url;
        }


        private static bool validateImageExtension(string extension)
        {
            string[] permittedExtensions = { ".png", ".jpg", ".jpeg", ".webp" };
            if (permittedExtensions.Contains(extension))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
