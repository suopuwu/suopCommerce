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
        public List<suopCommerce.Models.Image> images = new();
        public async Task<suopCommerce.Models.Image?[]> GetImagesAsync(int[]? ids)
        {
            return await BulkImageRetriever.FromListAsync(ids, false);
        }
        public void OnGet()
        {
            products = db.Products.ToList();
            images = db.Images.ToList();
            nextId = Uuid.GetUuid();
        }
    }
}

