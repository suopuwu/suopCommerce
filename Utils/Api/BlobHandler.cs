using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using suopCommerce.Models;
using System.ComponentModel;
using System.Security.Policy;

namespace SuopCommerce.Utils.Api
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
        public static async Task<List<int>> UploadImagesAsync(IFormFileCollection files)
        {
            string containerName = "images";
            List<int> images = new();
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
                    BlobHttpHeaders headers = new BlobHttpHeaders
                    {
                        ContentType = "image/" + extension[1..]
                    };
                    await blobClient.SetHttpHeadersAsync(headers);
                }
                Image tempImage = new();
                tempImage.Url = blobUrl;
                db.Images.Add(tempImage);
                db.SaveChanges();
                images.Add(tempImage.Id);
            }
            return images;
        }

        public static async Task<string> DeleteImageAsync(int id)
        {
            Image? imageToDelete = db.Images.Find(id);
            if (imageToDelete == null)
            {
                return "No such image within the database";
            }
            string url = imageToDelete.Url;
            string containerName = "images";
            string blobName = url.Split('/').Last();
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            if (await containerClient.DeleteBlobIfExistsAsync(blobName))
            {



                db.Images.Remove(imageToDelete);
                db.SaveChanges();

            }
            else
            {
                return "Failed to delete image, image does not exist in storage";
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
