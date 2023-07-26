using Azure.Core;
using suopCommerce.Models;
using System.Text.Json;

namespace SuopCommerce.Utils.Api
{
    public static class ProductDao
    {
        //updates a product if it exists, creates it otherwise
        public static async Task<string> Set(string name, string description, string category, double price, string[] tags, string[] extras, int[] images, int? id = null)
        {
            StoreContext db = new();

            Product product;
            if (id == null)
            {
                product = new Product();
            }
            else
            {
                try
                {

                    product = await db.Products.FindAsync(id) ?? throw new Exception("Product not found");
                }
                catch (Exception e)
                {
                    return JsonSerializer.Serialize(new { success = false, message = e.Message });
                }
            }
            product.Name = name;
            product.Description = description;
            product.Category = category;
            product.Price = price;
            product.Tags = tags;
            product.Extras = extras;
            product.Images = images;
            //todo add better addon and extra creator.

            try
            {
                if (id == null)
                {
                    await db.Products.AddAsync(product);
                }

                db.SaveChanges();
                return JsonSerializer.Serialize(new { success = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { success = false, message = ex.Message });
            }
        }
        public static async Task<string> Delete(int id, bool deleteImages)
        {
            StoreContext db = new();
            Product? productToDelete = db.Products.Find(id);
            if (productToDelete == null)
            {
                return "failed";
            }

            if (productToDelete.Images != null && deleteImages)
            {
                foreach (int imageId in productToDelete.Images)
                {
                    await ImageDao.DeleteImageAsync(imageId);
                }
            }


            db.Products.Remove(productToDelete);
            db.SaveChanges();
            return "deleted";
        }
        public static async Task<string> Get(int id)
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
    }
}
