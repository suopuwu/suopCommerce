using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using suopCommerce.Models;
using System.ComponentModel;
using System.Security.Policy;
using System.Text.Json;

namespace SuopCommerce.Utils.Api
{
    public static class ImageDao
    {
        public const string baseBlobUrl = "https://suopstorageone.blob.core.windows.net";
        public static StoreContext db;
        public static BlobServiceClient blobServiceClient;

        static ImageDao()
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
        public static async Task<string> UploadImagesAsync(IFormFileCollection files)
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
            return JsonSerializer.Serialize(new { data = images, success = (images.Count > 0)});
        }

        public static async Task<string> DeleteImageAsync(int id, bool deleteFromProduct = false)
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

            if (deleteFromProduct)
            {
                foreach(var product in db.Products)
                {
                    int pos = Array.IndexOf(product.Images ?? Array.Empty<int>(), id);
                    if(pos > -1)
                    {
                        var newArray = product.Images!.ToList();
                        newArray.RemoveAt(pos);
                        product.Images = newArray.ToArray();
                    }
                }
                db.SaveChanges();
            }

            
            return "deleted the image at url " + url;
        }

        public static async Task<string> GetImageAsync(int id) {//todo figure out a way to reduce the reusage of code like below
            StoreContext db = new();
            try
            {
                return JsonSerializer.Serialize(await db.Images.FindAsync(id) ?? throw new Exception("Image not found"));

            }
            catch (Exception ex)
            {
                return $"\"Error: {ex.Message}\"";
            }
        }

        public static async Task<string> UpdateImageAsync(int id, string? description)//todo make all api calls return json strings
        {
            StoreContext db = new();
            var image = await db.Images.FindAsync(id);
            if(image == null) {
                return JsonSerializer.Serialize(new { success = false, message = "Image not found."});
            }

            if(description == null) {
                return JsonSerializer.Serialize(new { success = false, message = "No description provided." });
            }

            image.Description = description;
            db.SaveChanges();
            return JsonSerializer.Serialize(new { success = true, newDescription = description });
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
