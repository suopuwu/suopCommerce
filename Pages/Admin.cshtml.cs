using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using suopCommerce.Models;
using SuopCommerce.Utils;
using System.Text.Json;

namespace SuopCommerce.Pages
{
    public class AdminModel : PageModel
    {
        private Product product = new();
        private StoreContext db { get; set; } = new();

        public string nextId = "";

        public void OnGet()
        {
            nextId = Uuid.GetUuid();
        }
        public void OnPost() {
            Console.WriteLine("test");
            if(Request.Form == null)
            {
                return;
            }
            product.Id = Request.Form["Id"];
            product.Name = Request.Form["Name"];
            product.Description = Request.Form["Description"];
            product.CategoryId = Request.Form["CategoryId"];
            product.Price = int.Parse(Request.Form["Price"]);
            product.Tags = Request.Form["Tags"];
            db.Products.Add(product);//todo move this into another file, protect from nulls
            db.SaveChanges();
            ViewData["debug"] = JsonSerializer.Serialize(product);
        }
    }
}
