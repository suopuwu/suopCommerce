using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SuopCommerce.Models
{
    public record CartItem(string Id, int Quantity, CartItem[] Children);
}
