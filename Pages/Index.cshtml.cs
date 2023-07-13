using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using suopCommerce.Models;
using SuopCommerce.Utils.Data;

namespace SuopCommerce.Pages
{
    public class IndexModel : ExtendedPageModel
    {
        //private readonly ILogger<IndexModel> _logger;

        //public IndexModel(ILogger<IndexModel> logger)
        //{
        //    _logger = logger;
        //}
        //todo look into making it preload images in the cart
        public List<Product> products = new();

        
        public void OnGet()
        {
            products = db.Products.OrderBy(e => e.Id).ToList();
        }
    }
}