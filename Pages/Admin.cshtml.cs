using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using suopCommerce.Models;
using SuopCommerce.Utils;
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

        public void OnGet()
        {
            nextId = Uuid.GetUuid();
        }

        public async void OnPostAsync()
        {
            Console.WriteLine("test");
            if (Request.Form == null)
            {
                return;
            }

            Product product = new();

            product.Id = Request.Form["Id"];
            product.Name = Request.Form["Name"];
            product.Description = Request.Form["Description"];
            product.CategoryId = Request.Form["CategoryId"];
            product.Price = int.Parse(Request.Form["Price"]);
            product.Tags = Request.Form["Tags"];//todo add tags, images, addons, category, just make sure that all fields are editable.
            db.Products.Add(product);//todo move this into another file, protect from nulls


            //todo move database code out of this file

            try
            {
                var blobs = await FileUpload.ImageAsync(Request.Form.Files);
                product.Images = blobs.ToArray();
                ViewData["debug"] = JsonSerializer.Serialize(product);

                db.SaveChanges();
                //todo make the debug string display when delayed due to the upload time on an image.

            }
            catch (Exception ex)
            {
                ViewData["debug"] = ex.Message;
            }
        }


    }
}

