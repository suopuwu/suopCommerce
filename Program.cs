using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stripe;
using SuopCommerce.Models;
using SuopCommerce.Pages;
using SuopCommerce.Utils;
using SuopCommerce.Utils.Data;
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
app.MapDelete("/api/products/{id}", ProductDao.Delete);
app.MapPost("/api/products", async (HttpContext context) =>
{
    return await ProductDao.Create(
                    context.Request.Form["Id"]!,
                    context.Request.Form["Name"]!,
                    context.Request.Form["Description"]!,
                    context.Request.Form["CategoryId"]!,
                    double.Parse(context.Request.Form["Price"]!),
                    context.Request.Form["Tags"]!,
                    context.Request.Form.Files);
});
app.MapGet("/api/products/{id}", ProductDao.Get);
app.MapPost("/api/products/{id}", async (HttpContext context, string id) =>
{
    return await ProductDao.Update(
                    id,
                    context.Request.Form["Name"]!,
                    context.Request.Form["Description"]!,
                    context.Request.Form["CategoryId"]!,
                    double.Parse(context.Request.Form["Price"]!),
                    context.Request.Form["Tags"]!);
});

app.MapPost("/api/checkout", async (HttpContext context) => {
    using StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8);
    string jsonData = await reader.ReadToEndAsync();
    var parsedData = JsonConvert.DeserializeObject<CartItem[]>(jsonData);
    if (parsedData == null)
    {
        context.Response.StatusCode = 400;
        return "Error: data is null";
    }
    return await PaymentIntentHandler.CreateAsync(parsedData);
});
app.UseAuthorization();
app.MapRazorPages();

app.Run();
