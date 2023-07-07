using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SuopCommerce.Models
{
    public record CartItem(int Id, int Quantity, CartItem[] Children);
}
