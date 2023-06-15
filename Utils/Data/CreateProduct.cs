using Azure.Core;
using suopCommerce.Models;
using System.Text.Json;

namespace SuopCommerce.Utils.Data
{
    public static class CreateProduct
    {
        public static async Task<String> Create(string id, string name, string description, string categoryId, double price, string[] tags, IFormFileCollection images)
        {
            StoreContext db = new();

            Product product = new();
            product.Id = id;
            product.Name = name;
            product.Description = description;
            product.CategoryId = categoryId;
            product.Price = price;
            product.Tags = tags;
            //todo add tags, images, addons, category, just make sure that all fields are editable.


            try
            {
                var blobs = await BlobHandler.UploadImageAsync(images);
                product.Images = blobs.ToArray();
                db.Products.Add(product);

                db.SaveChanges();
                return JsonSerializer.Serialize(product);

                //todo make the debug string display when delayed due to the upload time on an image.

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
