using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using suopCommerce.Models;

namespace SuopCommerce.Pages
{
    public class IndexModel : PageModel
    {
        //private readonly ILogger<IndexModel> _logger;

        //public IndexModel(ILogger<IndexModel> logger)
        //{
        //    _logger = logger;
        //}

        private StoreContext db { get; set; } = new();
        public int itemCount = 0;
        public List<Product> products = new();
        public void OnGet()
        {
            products = db.Products.ToList();
            itemCount = db.Products.Count();
        }
    }
}