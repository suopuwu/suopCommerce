using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Stripe;

namespace SuopCommerce.Utils
{
    public static class PaymentIntentApiController
    {
        public static string Create(PaymentIntentCreateRequest request)
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
            {
                Amount = CalculateOrderAmount(request.Items),
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            });

            return JsonSerializer.Serialize(new { clientSecret = paymentIntent.ClientSecret });
        }

        private static int CalculateOrderAmount(Item[] items)
        {
            // Replace this constant with a calculation of the order's amount
            // Calculate the order total on the server to prevent
            // people from directly manipulating the amount on the client
            return 1400;
        }

        public class Item
        {
            public string Id { get; set; }
        }

        public class PaymentIntentCreateRequest
        {
            public Item[] Items { get; set; }
        }
    }
}

