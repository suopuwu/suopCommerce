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

namespace SuopCommerce.Utils.Api
{
    public static class PaymentIntentHandler
    {
        public static async Task<string> CreateAsync(CartItem[] cart, string? successUrl, string? cancelUrl)
        {
            successUrl = successUrl ?? string.Empty;
            cancelUrl = cancelUrl ?? string.Empty;
            var lineItems = new List<SessionLineItemOptions>();
            using var db = new StoreContext();

            foreach (var CartItem in cart)
            {
                if (CartItem.Id == null) continue;
                var item = await db.Products.FindAsync(CartItem.Id);
                if (item == null) continue;
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Name,
                            Description = item.Description,
                            //Images = item.Images?.ToList(),todo fix this
                        },
                        UnitAmount = (long?)(item.Price * 100)
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
            };
            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return JsonConvert.SerializeObject(new { Location = session.Url });
        }
    }

}