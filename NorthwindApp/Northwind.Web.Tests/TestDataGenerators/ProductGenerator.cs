using System;
using System.Collections.Generic;
using Northwind.Model;
using Bogus;
using Bogus.DataSets;
namespace Northwind.Web.Tests.TestDataGenerators
{
    public class ProductGenerator : ITestDataGenerator<Product>
    {
        private readonly NorthwindContext? northwindContext;
        private Faker<Product> faker;

        public ProductGenerator(NorthwindContext? context = null)
        {
            northwindContext = context;

            faker = new Faker<Product>()
                .StrictMode(false)
                .RuleFor(p => p.ProductName, f => f.Commerce.ProductName())
                .RuleFor(p => p.UnitPrice, f => decimal.Parse(f.Commerce.Price()))
                .RuleFor(p => p.UnitsInStock, f => f.Random.Short(min: 0))
                .RuleFor(p => p.UnitsOnOrder, f => f.Random.Short(min: 0))
                .RuleFor(p => p.ReorderLevel, f => f.Random.Short(min: 0))
                .RuleFor(p => p.Discontinued, f => f.Random.Bool());
        }

        public ProductGenerator WithName(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException(nameof(productName),
                    "Явно указанное имя категории не должно быть пустым");

            faker.RuleFor(p => p.ProductName, productName);
            return this;
        }

        public ProductGenerator WithCategory(Category category)
        {
            faker.RuleFor(p => p.CategoryId, category.CategoryId)
                 .RuleFor(p => p.Category, category);
            return this;
        }

        public ProductGenerator WithPrice(decimal price)
        {
            if (price <= 0)
                throw new ArgumentOutOfRangeException(nameof(price),
                    "Цена должна быть больше 0");
            faker.RuleFor(p => p.UnitPrice, price);
            return this;
        }

        public Product Generate()
        {
            var product = faker.Generate();
            northwindContext?.Products.Add(product);
            return product;
        }

        public IEnumerable<Product> Generate(int count)
        {
            var products = faker.Generate(count);
            northwindContext?.Products.AddRange(products);
            return products;
        }
    }
}
