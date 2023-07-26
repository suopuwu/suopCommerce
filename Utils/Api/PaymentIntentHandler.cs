using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Stripe;
using Azure;
using Stripe.Checkout;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;
using SuopCommerce.Models;
using suopCommerce.Models;
using SuopCommerce.Utils.Data;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace SuopCommerce.Utils.Api
{
    public static class PaymentIntentHandler
    {

        private static long CalculatePrice(CartItem item, suopCommerce.Models.Product product)
        {
            using var db = new StoreContext();

            //base price
            long price = (long)(product.Price * 100);

            //add addons
            foreach (var addonId in item.Children)
            {
                var addon = db.Products.Find(addonId.Id);
                if (addon == null) continue;
                price += (long)(addon.Price * 100);
            }

            foreach (var customization in item.Customization)
            {
                //ensure it is just a key and value
                //customization[0] = key
                //customization[1] = value
                if (customization.Length != 2) continue;
                var chosenExtra = Extras.GetExtraFromIdInProduct(customization[0], product);
                switch (chosenExtra.Type)
                {
                    case Extras.Types.PerLetter:
                        price += (long)((chosenExtra.Cost * customization[1].Length) * 100);
                        break;
                    case Extras.Types.Radio:
                        var costOfThisOption = 0.0;
                        chosenExtra.RadioOptions?.TryGetValue(customization[1], out costOfThisOption);
                        price += (long)(costOfThisOption * 100);
                    break;
                    case Extras.Types.Invalid:
                        break;
                }

            }
            return price;
        }

        public static string CreateDescription(CartItem item, suopCommerce.Models.Product product)
        {
            using var db = new StoreContext();

            var description = new StringBuilder();
            foreach (var field in item.Customization)
            {
                var chosenExtra = Extras.GetExtraFromIdInProduct(field[0], product);
                switch (chosenExtra.Type)
                {
                    default:
                        description.Append(chosenExtra.Text).Append(": ").Append(field[1]).Append(", ");
                        break;
                    case Extras.Types.TextField:
                        description.Append(chosenExtra.Text).Append(": \"").Append(field[1]).Append("\", ");
                        break;
                    case Extras.Types.Invalid:
                        break;
                }
            }
            //todo entering the cart from below makes it flash... sometimes??
            foreach (var addonId in item.Children)
            {
                var addon = db.Products.Find(addonId.Id);
                if (addon == null) continue;
                description.Append(addon.Name).Append(" added, ");
            }
            description.Remove(description.Length - 2, 1);
            return description.ToString();
        }

        public static async Task<string> CreateAsync(CartItem[] cart, string? successUrl, string? cancelUrl)
        {
            successUrl ??= string.Empty;
            cancelUrl ??= string.Empty;
            var lineItems = new List<SessionLineItemOptions>();
            using var db = new StoreContext();

            foreach (var cartItem in cart)
            {
                var item = await db.Products.FindAsync(cartItem.Id);
                var images = await BulkImageRetriever.FromListAsync(item.Images);
                List<string> imageUrls = new();
                foreach (var image in images)
                {
                    if (image == null) continue;
                    imageUrls.Add(image.Url);
                }
                if (item == null) continue;
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Name,
                            Description = CreateDescription(cartItem, item),
                            Images = imageUrls,
                            Metadata = new Dictionary<string, string> { { "lskdj", "lksdjf" } }
                        },
                        UnitAmount = CalculatePrice(cartItem, item)
                    },
                    Quantity = 1,
                });
            }
            var options = new SessionCreateOptions
            {
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                ShippingAddressCollection = new SessionShippingAddressCollectionOptions
                {
                    AllowedCountries = new List<string> {
                        "US"
                    }
                }
            };
            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return JsonConvert.SerializeObject(new { success = true, Location = session.Url });
        }
    }

}