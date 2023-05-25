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

        public void OnGet()
        {
            itemCount = db.Products.Count();
        }
    }
}