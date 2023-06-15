using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using suopCommerce.Models;

namespace SuopCommerce.Pages.Products
{
    public class ProductModel : PageModel
    {
        private StoreContext db { get; set; } = new();
        public Product product = new();
        public bool redirect = false;
        public void OnGet()
        {//todo figure out how or if this should be async.
            try
            {
                product = db.Products.Find(RouteData.Values["id"]) ?? throw new Exception("Product does not exist");
            } catch
            {
                HttpContext.Response.Redirect("/404");
            }

        }
    }
}
