using Stripe;
using SuopCommerce.Models;
using SuopCommerce.Utils.Api;
using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using SuopCommerce.Utils;
using System.Text.Json;
using Newtonsoft.Json;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

void ConfigureStripe()
{
    const string secretName = "stripeApiKey";
    var keyVaultName = "suopCommerceSecrets";
    var kvUri = $"https://{keyVaultName}.vault.azure.net";

    var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
    StripeConfiguration.ApiKey = client.GetSecret(secretName).Value.Value;
}

ConfigureStripe();

//admin api calls

//todo make all admin api calls have admin in the url.
app.MapPost("/api/setPassword/{newPassword}", (HttpContext context, string newPassword) =>
{
    return AuthenticationUtil.AuthenticateApiCall(context) ?? AuthenticationUtil.SetPassword(newPassword);
});

app.MapDelete("/api/products/{id}", async (HttpContext context, string id) =>
{
    return AuthenticationUtil.AuthenticateApiCall(context) ??
        await ProductDao.Delete(int.Parse(id), context.Request.Headers["delete-images"] == "true");
});

app.MapPost("/api/products", async (HttpContext context) =>
{

    return AuthenticationUtil.AuthenticateApiCall(context) ?? 
        await ProductDao.Set(
                  context.Request.Form["Name"]!,
                  context.Request.Form["Description"]!,
                  context.Request.Form["Category"]!,
                  double.Parse(context.Request.Form["Price"]!),
                  ((string?)context.Request.Form["Tags"] ?? "").Replace(" ", "").Split(",").ToArray(),
                  ((string?)context.Request.Form["Extras"] ?? "").Split(",").Select(extra => extra.Trim()).ToArray(),
                  ((string?)context.Request.Form["Images"] ?? "").Replace(" ", "").Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()
                  );
});

app.MapPost("/api/products/{id}", async (HttpContext context, int id) =>
{
    return AuthenticationUtil.AuthenticateApiCall(context) ??
        await ProductDao.Set(
                  context.Request.Form["Name"]!,
                  context.Request.Form["Description"]!,
                  context.Request.Form["Category"]!,
                  double.Parse(context.Request.Form["Price"]!),
                  ((string?)context.Request.Form["Tags"] ?? "").Replace(" ", "").Split(",").ToArray(),
                  ((string?)context.Request.Form["Extras"] ?? "").Split(",").Select(extra => extra.Trim()).ToArray(),
                  ((string?)context.Request.Form["Images"] ?? "").Replace(" ", "").Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(),
                  id
                  );
});//todo global images and text, ie color palette and info about all products


app.MapPost("/api/images", async (HttpContext context) =>
{
    return AuthenticationUtil.AuthenticateApiCall(context) ?? 
        await ImageDao.UploadImagesAsync(context.Request.Form.Files);
});
app.MapDelete("/api/images", async (HttpContext context) =>
{
    return AuthenticationUtil.AuthenticateApiCall(context) ?? 
        await ImageDao.DeleteImageAsync(int.Parse(context.Request.Headers["id"]!), context.Request.Headers["delete-from-product"] == "true");
});


app.MapPost("/api/images/{id}", async (HttpContext context, int id) =>
{
    return AuthenticationUtil.AuthenticateApiCall(context) ?? 
        await ImageDao.UpdateImageAsync(id, context.Request.Form["Description"]);
});





//public api calls

app.MapGet("/api/images/{id}", async (int id) =>
{
    return await ImageDao.GetImageAsync(id);
});//todo see about moving these to a different file

app.MapPost("/api/checkout", async (HttpContext context) =>
{
    try
    {
        using StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8);
        string jsonData = await reader.ReadToEndAsync();
        var parsedData = JsonConvert.DeserializeObject<CartItem[]>(jsonData);
        if (parsedData == null)
        {
            context.Response.StatusCode = 400;
            return JsonConvert.SerializeObject(new { success = false, message = "Error: data is null" });
        }
        var headers = context.Request.Headers;

        return await PaymentIntentHandler.CreateAsync(parsedData, headers["successUrl"], headers["cancelUrl"]);

    }
    catch (Exception ex)
    {
        return JsonConvert.SerializeObject(new { success = false, message = ex.Message });

    }
});

app.MapGet("/api/products/{id}", ProductDao.Get);

app.UseAuthorization();
app.MapRazorPages();

app.Run();
