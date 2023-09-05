using Newtonsoft.Json;
using SuopCommerce.Models;
using SuopCommerce.Utils.Data;

namespace SuopCommerce.Pages
{
    public class checkoutModel : ExtendedPageModel
    {
        public CartItem[] cartItems = { };
        public double totalPrice = 0;
        public async Task OnGetAsync()
        {
            cartItems = JsonConvert.DeserializeObject<CartItem[]>(Request?.Cookies["cart"] ?? "[]")?.Where(item => item != null).ToArray() ?? Array.Empty<CartItem>();
            foreach(var item in cartItems)
            {
                //if performance is a problem, refactor so this part doesn't run twice. (one in here, one in the cshtml file)
                var product = await db.Products.FindAsync(item.Id);
                if (product == null) continue;

                totalPrice += ((double) GetPriceFromProduct(item, product)) / 100;
            }
        }
    }
}
