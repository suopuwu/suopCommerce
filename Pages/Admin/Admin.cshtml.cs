using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using suopCommerce.Models;
using SuopCommerce.Utils;
using SuopCommerce.Utils.Data;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SuopCommerce.Pages.Admin
{
    public class AdminModel : PageModel
    {

        private StoreContext db { get; set; } = new();

        public string nextId = "";
        public List<Product> products = new();
        public void OnGet()
        {
            products = db.Products.ToList();
            nextId = Uuid.GetUuid();
        }

        public async void OnPostDelete()
        {
            ViewData["debug"] = "delete pressed";
        }

        public async void OnPostAsync()
        {
            Console.WriteLine("test");
            if (Request.Form == null)
            {
                return;
            }

            try
            {
                ViewData["debug"] = await CreateProduct.Create(
                    Request.Form["Id"]!,
                    Request.Form["Name"]!,
                    Request.Form["Description"]!,
                    Request.Form["CategoryId"]!,
                    double.Parse(Request.Form["Price"]!),
                    Request.Form["Tags"]!,
                    Request.Form.Files);
            } catch
            {
                ViewData["debug"] = "One or more required fields were missing";
            }//todo make the ui for adding or editing products unified for code reusage.

        }


    }
}

