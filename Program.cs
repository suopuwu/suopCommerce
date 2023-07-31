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
app.MapDelete("/api/products/{id}", async (HttpContext context, string id) =>
{
  return await ProductDao.Delete(int.Parse(id), context.Request.Headers["delete-images"] == "true");
});
app.MapPost("/api/products", async (HttpContext context) =>
{

  return await ProductDao.Set(
                  context.Request.Form["Name"]!,
                  context.Request.Form["Description"]!,
                  context.Request.Form["Category"]!,
                  double.Parse(context.Request.Form["Price"]!),
                  ((string?)context.Request.Form["Tags"] ?? "").Replace(" ", "").Split(",").ToArray(),
                  ((string?)context.Request.Form["Extras"] ?? "").Split(",").Select(extra => extra.Trim()).ToArray(),
                  ((string?)context.Request.Form["Images"] ?? "").Replace(" ", "").Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()
                  );
});
app.MapGet("/api/products/{id}", ProductDao.Get);
app.MapPost("/api/products/{id}", async (HttpContext context, int id) =>
{
  return await ProductDao.Set(
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

app.MapPost("/api/images", async (HttpContext context) =>
{
  return await ImageDao.UploadImagesAsync(context.Request.Form.Files);
});
app.MapDelete("/api/images", async (HttpContext context) =>
{
  return await ImageDao.DeleteImageAsync(int.Parse(context.Request.Headers["id"]!), context.Request.Headers["delete-from-product"] == "true");
});

app.MapGet("/api/images/{id}", async (int id) =>
{
  return await ImageDao.GetImageAsync(id);
});//todo see about moving these to a different file

app.MapPost("/api/images/{id}", async (HttpContext context, int id) =>
{
  return await ImageDao.UpdateImageAsync(id, context.Request.Form["Description"]);
});

app.UseAuthorization();
app.MapRazorPages();

app.Run();
