using suopCommerce.Models;
using SuopCommerce.Utils.Data;

namespace SuopCommerce.Pages.Products
{
    public class ProductModel : ExtendedPageModel
    {
        public Product? product = new();
        public Image[] images = Array.Empty<Image>();
        public bool redirect = false;
        public string debug = "";
        public async Task OnGetAsync()
        {
            product = await db.Products.FindAsync(int.Parse(RouteData.Values["id"]!.ToString()!));
            if(product == null)
            {
                HttpContext.Response.Redirect("/404");

            }
            images = await GetImageArrayFromIdsAsync(product.Images);
        }
    }
}
