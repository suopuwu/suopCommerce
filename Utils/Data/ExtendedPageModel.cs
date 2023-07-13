using Microsoft.AspNetCore.Mvc.RazorPages;
using suopCommerce.Models;

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

        public string formatMoney(double d) => MoneyFormatter.formatDouble(d);
    }
}
