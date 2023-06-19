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
    }
}

