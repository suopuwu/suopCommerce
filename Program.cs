using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using SuopCommerce.Pages;
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
//app.MapGet("products/{productId}", (string productId) =>
//{
//    return new { productId };
//}); api functions
//app.MapGet("products/{productId}", (string productId) => );
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
app.UseAuthorization();
app.MapRazorPages();

app.Run();
