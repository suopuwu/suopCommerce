using Azure.Core;
using suopCommerce.Models;
using System.Text.Json;

namespace SuopCommerce.Utils.Data
{
    public static class ProductDao
    {
        public static async Task<string> Create(string id, string name, string description, string categoryId, double price, string[] tags, IFormFileCollection images)
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
            //todo make it remove metadata

            try
            {
                var blobs = await BlobHandler.UploadImageAsync(images);
                product.Images = blobs.ToArray();
                db.Products.Add(product);

                db.SaveChanges();
                return "success";

                //todo make the debug string display when delayed due to the upload time on an image.

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static async Task<string> Delete(string id)
        {
            StoreContext db = new();
            Product? productToDelete = db.Products.Find(id);
            if (productToDelete == null)
            {
                return "failed";
            }

            if (productToDelete.Images != null)
            {
                foreach (string imageUrl in productToDelete.Images)
                {
                    await BlobHandler.DeleteImageAsync(imageUrl);
                }
            }


            db.Products.Remove(productToDelete);
            db.SaveChanges();
            return "deleted";
        }
        public static async Task<string> Get(string id)
        {
            StoreContext db = new();
            try
            {
                return JsonSerializer.Serialize(await db.Products.FindAsync(id) ?? throw new Exception("Product not found"));

            }
            catch (Exception ex)
            {
                return $"\"Error: {ex.Message}\"";
            }
        }

        public static async Task<string> Update(string id, string name, string description, string categoryId, double price, string[] tags)
        {//todo editing the id is buggy, you cannot edit images, tags to not work as well
            StoreContext db = new();
            Product? product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return "\"No such product.\"";
            }
            product.Id = id;
            product.Name = name;
            product.Description = description;
            product.CategoryId = categoryId;
            product.Price = price;
            product.Tags = tags;
            await db.SaveChangesAsync();
            return "success";
        }
    }
}
