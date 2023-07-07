using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stripe;
using SuopCommerce.Models;
using SuopCommerce.Pages;
using SuopCommerce.Utils.Api;
using System.Text;

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

StripeConfiguration.ApiKey = "sk_test_51NMGmDHGAiSJzSGYXfof3JxlLvX0oWg8Hh6I23NzCnLiUSEgRby5Lr23ZIuv9wZOUnwOzEsINXfdV73euSefLink00StTj2tFH";
app.MapDelete("/api/products/{id}", async (string id) =>
{
    return await ProductDao.Delete(int.Parse(id));
});
app.MapPost("/api/products", async (HttpContext context) =>
{
    
    return await ProductDao.Create(
                    context.Request.Form["Name"]!,
                    context.Request.Form["Description"]!,
                    context.Request.Form["CategoryId"]!,
                    double.Parse(context.Request.Form["Price"]!),
                    ((string?)context.Request.Form["Tags"] ?? "").Split(",").Select(p => p.Trim()).ToArray(),
                    ((string?)context.Request.Form["Extras"] ?? "").Split(",").Select(p => p.Trim()).ToArray(),
                    ((string?)context.Request.Form["Addons"] ?? "").Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(),
                    context.Request.Form.Files);
});
app.MapGet("/api/products/{id}", ProductDao.Get);
app.MapPost("/api/products/{id}", async (HttpContext context, string id) =>
{
    string? tags = context.Request.Form["Tags"];//make sure all image related stuff is deleted on removal
    tags ??= "";
    return await ProductDao.Update(
                    int.Parse(id),
                    context.Request.Form["Name"]!,
                    context.Request.Form["Description"]!,
                    context.Request.Form["CategoryId"]!,
                    double.Parse(context.Request.Form["Price"]!),
                    ((string?)context.Request.Form["Tags"] ?? "").Split(",").Select(p => p.Trim()).ToArray(),
                    ((string?)context.Request.Form["Extras"] ?? "").Split(",").Select(p => p.Trim()).ToArray(),
                    ((string?)context.Request.Form["Addons"] ?? "").Split(",").Select(p => int.Parse(p.Trim())).ToArray());
});

app.MapPost("/api/checkout", async (HttpContext context) =>
{
    using StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8);
    string jsonData = await reader.ReadToEndAsync();
    var parsedData = JsonConvert.DeserializeObject<CartItem[]>(jsonData);
    if (parsedData == null)
    {
        context.Response.StatusCode = 400;
        return "Error: data is null";
    }
    var headers = context.Request.Headers;

    return await PaymentIntentHandler.CreateAsync(parsedData, headers["successUrl"], headers["cancelUrl"]);
});

app.MapPost("/api/images", async (HttpContext context) =>
{
    return await BlobHandler.UploadImagesAsync(context.Request.Form.Files);
});
app.MapDelete("/api/images", async (HttpContext context) =>
{
    return await BlobHandler.DeleteImageAsync(int.Parse(context.Request.Headers["id"]!));
});


app.UseAuthorization();
app.MapRazorPages();

app.Run();
