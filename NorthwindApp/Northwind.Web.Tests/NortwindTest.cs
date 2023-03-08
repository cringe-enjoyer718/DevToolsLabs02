using System;
using AngleSharp;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Northwind.Model;
using Northwind.Web.Models;

namespace Northwind.Web.Tests
{
    [TestClass]
    public class NortwindTest
    {
        [TestMethod]
        public async Task Home_ContainsAllLinks()
        {
            var client = GetHttpClient();

            var response = await client.GetStringAsync("/");
            var links = GetHomeLinks(response);

            links.Should().Contain(new string[] { "/", "/categories" });
        }

        [TestMethod]
        public async Task Index_ReturnViewResult_WithAllCategories()
        {
            var client = GetHttpClient();
            var context = new NorthwindContext();

            var response = await client.GetStringAsync("/categories");
            var result = HtmlParser.GetResultCategories(response);

            result.Should().BeEquivalentTo(context.Categories,
                options => options
                .Excluding(c => c.Picture)
                .Excluding(c => c.Products));
        }

        [TestMethod]
        public async Task Details_ReturnViewResult_WithCategoryDetails()
        {
            var client = GetHttpClient();
            var contxt = new NorthwindContext();

            var category = contxt.Categories.Include(c => c.Products).First();

            var response = await client.GetStringAsync($"/categories/details/{category.CategoryId}");
            var categoryViewModel = HtmlParser.GetCategoryFromDetails(response);

            categoryViewModel.Should().BeEquivalentTo(category.ToViewModel(),
                options => options
                .Including(c => c.CategoryName)
                .Including(c => c.Description)
                .Including(c => c.ProductsCount)
                .Including(c => c.ProductsNames));

            categoryViewModel.ProductsNames.Should().BeEquivalentTo(
                category.Products
                .Take(3).Select(p => p.ProductName));
        }

        private static HttpClient GetHttpClient()
        {
            return new HttpClient() { BaseAddress = new Uri("http://localhost:5129/") };
        }

        private static IEnumerable<string>? GetHomeLinks(string htmlSource)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(htmlSource)).Result;

            return document.Links.Select(link => link.Attributes["href"]!.Value.ToLower());
        }
    }
}
