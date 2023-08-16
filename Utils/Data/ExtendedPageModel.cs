using Markdig;
using Microsoft.AspNetCore.Mvc.RazorPages;
using suopCommerce.Models;
using SuopCommerce.Models;
using SuopCommerce.Utils.Api;

namespace SuopCommerce.Utils.Data
{
    public class ExtendedPageModel : PageModel
    {
        public StoreContext db { get; set; } = new();
        public async Task<string> GetImageUrlAsync(int id)
        {
            var image = await db.Images.FindAsync(id) ?? new Image();
            return image.Url;
        }

        #pragma warning disable CS8619 //The below call returns a pruned list, so it will never be null
        public async Task<Image[]> GetImageArrayFromIdsAsync(int[] list) => await BulkImageRetriever.FromListAsync(list);
#pragma warning restore CS8619

        public string FormatMoney(double d) => MoneyFormatter.formatDouble(d);

        public string MarkdownToHtml(string markdown)
        {
            return Markdown.ToHtml(markdown);
        }

        public long GetPriceFromProduct(CartItem item, Product product) => PaymentIntentHandler.CalculatePrice(item, product);
        public string GetDescriptionFromProduct(CartItem item, Product product) => PaymentIntentHandler.CreateDescription(item, product) ?? "";
    }
}
