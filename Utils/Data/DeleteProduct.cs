using suopCommerce.Models;

namespace SuopCommerce.Utils.Data
{
    public static class DeleteProduct
    {
        public static async Task<string> Delete(string id)
        {
            StoreContext db = new();
            Product productToDelete = db.Products.Find(id);
            if (productToDelete == null)
            {
                return $"No product with id {id} to delete.";
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
            return $"Product with id {id} deleted.";
        }
    }
}
