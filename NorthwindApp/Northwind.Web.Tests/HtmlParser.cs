using AngleSharp;
using AngleSharp.Dom;
using Northwind.Model;
using Northwind.Web.Models;
using System.Collections;

namespace Northwind.Web.Tests
{
    public class HtmlParser
    {
        public static IEnumerable<Category> GetResultCategories(string htmlSource)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(htmlSource)).Result;

            foreach (var categoryRow in document.QuerySelectorAll("tr[data-tid|='category-row']"))
            {
                var id = categoryRow.GetAttribute("data-tid")?.Split("-").Last();
                var name = categoryRow.QuerySelector("td[data-tid='category-name']")?.Text().Trim();
                var description = categoryRow.QuerySelector("td[data-tid='category-description']")?.Text().Trim();

                yield return new Category
                {
                    CategoryId = int.Parse(id ?? "-1"),
                    CategoryName = name ?? "",
                    Description = description,
                    Picture = null,
                };
            }
        }
        public static IEnumerable<string> GetValidationErrorMessages(string htmlSource)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(htmlSource)).Result;

            foreach (var element in document.QuerySelectorAll("span.field-validation-error"))
            {
                yield return element.InnerHtml;
            }
        }
        public static IEnumerable<string> GetAnotherValidationErrorMessages(string htmlSource)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(htmlSource)).Result;

            foreach (var element in document.QuerySelectorAll("[class*=validation]"))
            {
                yield return element.Text();
            }
        }
        public static CategoryViewModel GetCategoryFromDetails(string htmlSource)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(htmlSource)).Result;

            var category = document.QuerySelector("dl");
            var name = category?.QuerySelector("dd[data-tid='category-name']")?
                .Text().Trim();
            var description = category?.QuerySelector("dd[data-tid='category-description']")?
                .Text().Trim();
            var productsCount = Convert.ToInt32(category?.QuerySelector("dd[data-tid='category-products-count']")?
                .Text().Trim());
            var productsNames = category?.QuerySelector("dd[data-tid='category-products']")?
                .Text().Trim();
            productsNames = productsNames?.Substring(0, productsNames.Length - 3);

            var products = new List<Product>(productsCount);
            ArrayList arrayList = new ArrayList(2);

            return new CategoryViewModel
            {
                CategoryName = name ?? "",
                Description = description,
                ProductsCount = productsCount,
                ProductsNames = productsNames?.Split(", "),
            };
        }

    }
}
