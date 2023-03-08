using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Northwind.Model;
using Microsoft.Extensions.DependencyInjection;
using AngleSharp;
using FluentAssertions;
using Northwind.Web.Tests.TestDataGenerators;

using AngleSharp.Dom;


namespace Northwind.Web.Tests
{
    [TestClass]
    public class CategoriesControllerIntegrationTesting
    {
        private const string AspNetVerificationTokenName = "__RequestVerificationToken";

        [TestMethod]
        public async Task Index_ReturnViewResult_WithAllCategories()
        {
            // Создаем пустой контекст EF в памяти и заносим 10 категорий
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var categoryGenerator = new CategoryGenerator(context);
            var categories = categoryGenerator.Generate(10).ToList();
            context.SaveChanges();

            // Запускаем наше приложение на основе созданного и заполненного 
            // контекста и получаем HTTP клиент, который будет к этому приложению
            // обращаться
            var client = GetTestHttpClient(
                () => NorthwindContextHelpers.GetInMemoryContext());

            // Делаем GET запрос к списку категорий
            var response = await client.GetStringAsync("/categories");

            // Парсим полученную HTML, достаем данные о категориях
            var result = GetResultCategories(response).ToList();

            // Сверяем полученные в запросе и созданные ранее категории
            result.Should().BeEquivalentTo(categories, 
                options => options
                    .Excluding(c => c.Products)
                    .Excluding(c => c.Picture));
        }

        [TestMethod]
        public async Task Create_AddNewCategory_WithoutPicture_AsUrlEncodedForm_And_RedirectToList()
        {
            // Создаем пустой контекст и 1 категорию, но в базу её не сохраняем
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var category = new CategoryGenerator().Generate();

            // Запускаем приложение и получаем клиент, но с опцией, что он
            // не будет автоматически выполнять Redirect (чтобы мы могли проверить реальный
            // ответ на наше запрос)
            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(), 
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            // Обращаемся к форме создания новой категории, только чтобы получить
            // верификационный токен
            var createForm = await client.GetStringAsync("/categories/create");
            var verificationToken = GetRequestVerificationToken(createForm);

            // Формируем запрс, как если бы отправлялась ранее полученная форма
            var formContent = new FormUrlEncodedContent(
                new Dictionary<string, string> {
                    [nameof(Category.CategoryName)] = category.CategoryName,
                    [nameof(Category.Description)] = category.Description,
                    [AspNetVerificationTokenName] = verificationToken
                });

            // Получаем ответ и достаем из базы только что созданную категорию
            var response = await client.PostAsync("/categories/create", formContent);
            context = NorthwindContextHelpers.GetInMemoryContext();
            var newCategory = context.Categories.First();

            // Проверяем, что в качестве ответа нам пришел редирект на список категорий
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Redirect);
            response.Headers.Location.Should().Be("/Categories");

            // Проверяем, что категория из базы совпадает с тестовой
            newCategory.Should().BeEquivalentTo(category,
                options => options
                    .Including(c => c.CategoryName)
                    .Including(c => c.Description));
        }

        [TestMethod]
        public async Task Create_AddNewCategory_WithPicture_AsMultipartForm_And_RedirectToList()
        {
            // Всё делаем аналогично тесту выше, кроме формирования запроса
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var category = new CategoryGenerator().Generate();

            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var createForm = await client.GetStringAsync("/categories/create");
            var verificationToken = GetRequestVerificationToken(createForm);

            // Чтобы можно было отправить сразу тело файла картинки исползуем
            // multipart/form-data запрос
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent(category.CategoryName), nameof(Category.CategoryName));
            multipartContent.Add(new StringContent(category.Description), nameof(Category.Description));
            multipartContent.Add(new StringContent(verificationToken), AspNetVerificationTokenName);
            multipartContent.Add(new ByteArrayContent(category.Picture), "Picture", "picture.jpg");

            // Получаем и счверяем результат как в предыдущем тесте, только сверяем еще и картинку 
            var response = await client.PostAsync("/categories/create", multipartContent);
            context = NorthwindContextHelpers.GetInMemoryContext();
            var newCategory = context.Categories.First();

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Redirect);
            response.Headers.Location.Should().Be("/Categories");
            newCategory.Should().BeEquivalentTo(category,
                options => options
                    .Including(c => c.CategoryName)
                    .Including(c => c.Description)
                    .Including(c => c.Picture));
        }

        [TestMethod]
        public async Task Create_Should_Not_Create_Category_When_CategoryName_Is_Empty( )
        {
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var category = new CategoryGenerator().Generate();

            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var createForm = await client.GetStringAsync("/categories/create");
            var verificationToken = GetRequestVerificationToken(createForm);

            var formContent = new FormUrlEncodedContent(
               new Dictionary<string, string>
               {
                   [nameof(Category.CategoryName)] = null,
                   
                   [AspNetVerificationTokenName] = verificationToken
               });
            var response = await client.PostAsync("/categories/create", formContent);
            context = NorthwindContextHelpers.GetInMemoryContext();
           
            var html = await response.Content.ReadAsStringAsync();
            var errors = GetValidationErrorMessages(html);
            errors.Should().HaveCount(1);
            errors.First().Should().BeEquivalentTo("The Название field is requared");
            context.Categories.Should().BeEmpty();

        }

        [TestMethod]
        public async Task Delete_ShouldNotDeleteCategory_WhenCategoryContainsProducts()
        {
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var categoryToDelete = new CategoryGenerator(context)
                .WithProducts(new ProductGenerator(), 3).Generate();
            await context.SaveChangesAsync();

            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var createForm = await client.GetStringAsync($"/categories/delete/{categoryToDelete.CategoryId}");
            var verificationToken = GetRequestVerificationToken(createForm);

            var formData = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                [nameof(Category.CategoryId)] = categoryToDelete.CategoryId.ToString(),
                [AspNetVerificationTokenName] = verificationToken,
            });

            var response = await client
                .PostAsync($"/categories/delete/{categoryToDelete.CategoryId}", formData);
            var html = await response.Content.ReadAsStringAsync();
            var ErrorMessages = HtmlParser.GetAnotherValidationErrorMessages(html);
            ErrorMessages.Should().HaveCountGreaterThanOrEqualTo(1);
            ErrorMessages.First().Should().Contain("Нельзя удалять категории с привязанными товарами!");
            context = NorthwindContextHelpers.GetInMemoryContext();

            context.Categories.FirstOrDefault()
                .Should().BeEquivalentTo(categoryToDelete,
                options => options
                .Including(c => c.CategoryId)
                .Including(c => c.CategoryName)
                .Including(c => c.Description)
                .Including(c => c.Picture));
        }


        [TestMethod]
        public async Task Edit_Doesnt_Edit_Category_WhenSomeInformationIsNotRight_And_ShowError()
        {
            var context = NorthwindContextHelpers.GetInMemoryContext();
            var NewCategory = new CategoryGenerator(context).Generate();
            await context.SaveChangesAsync();

            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var create = await client.GetStringAsync($"/categories/edit/{NewCategory.CategoryId}");
            
            var NewContent = new MultipartFormDataContent();
            NewContent.Add(new StringContent(NewCategory.CategoryId.ToString()),
                nameof(Category.CategoryId));
            NewContent.Add(new StringContent(string.Empty), nameof(NewCategory.CategoryName));


            var response = await client.PostAsync($"/categories/edit/{NewCategory.CategoryId}", NewContent);
            var html = await response.Content.ReadAsStringAsync();
            var errors = GetValidationErrorMessages(html);

            errors.Should().HaveCount(1);
            errors.First().Should().BeEquivalentTo("The Название field is requared");
            context = NorthwindContextHelpers.GetInMemoryContext();
            var category = await context.Categories.FindAsync(NewCategory.CategoryId);
            category.Should().BeEquivalentTo(NewCategory,
                options => options
                .Including(c => c.CategoryId)
                .Including(c => c.CategoryName)
                .Including(c => c.Description)
                .Including(c => c.Picture));
        }

        [TestMethod]
        public async Task Edit_Return_NotFound_When_Category_no_Exist()
        {
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var category = new CategoryGenerator(context).Generate();
            await context.SaveChangesAsync();
            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var createForm = await client.GetStringAsync($"/categories/edit/{category.CategoryId}");
            var verificationToken = GetRequestVerificationToken(createForm);

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent("2"),
                nameof(Category.CategoryId));
            multipartContent.Add(new StringContent(category.CategoryName), nameof(Category.CategoryName));
            multipartContent.Add(new StringContent(category.Description!), nameof(Category.Description));
            multipartContent.Add(new StringContent(verificationToken), AspNetVerificationTokenName);
            multipartContent.Add(new ByteArrayContent(category.Picture!), nameof(Category.Picture), "picture.jpg");

            var response = await client.PostAsync("categories/edit/2", multipartContent);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Image_ReturnError_IfCategoryDoesntHavePicture()
        {
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var category = new CategoryGenerator(context).WithPicture(null!).Generate();
            await context.SaveChangesAsync();

            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext());
            var response = await client.GetAsync($"/categories/image/{category.CategoryId}");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Delete_Category_if_It_is_Empty()
        {
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var categoryToDelete = new CategoryGenerator(context).Generate();
            await context.SaveChangesAsync();
            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var createForm = await client.GetStringAsync($"/categories/delete/{categoryToDelete.CategoryId}");
            var verificationToken = GetRequestVerificationToken(createForm);


            var formData = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                [nameof(Category.CategoryId)] = categoryToDelete.CategoryId.ToString(),
                [AspNetVerificationTokenName] = verificationToken,
            });

            var response = await client
                .PostAsync($"/categories/delete/{categoryToDelete.CategoryId}", formData);

            context = NorthwindContextHelpers.GetInMemoryContext();

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Redirect);
            response.Headers.Location.Should().Be("/Categories");

            context.Categories.Should().BeEmpty();
        }
        private static HttpClient GetTestHttpClient(
            Func<NorthwindContext>? context = null,
            WebApplicationFactoryClientOptions? clientOptions = null
            )
        {
            var factory = new WebApplicationFactory<Program>();
            if (context != null)
            {
                factory = factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddScoped<NorthwindContext>(services => context());
                    });
                });
            }

            var client = clientOptions != null
                ? factory.CreateClient(clientOptions)
                : factory.CreateClient();

            return client;
        }
        
        private static string GetRequestVerificationToken(string htmlSource)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(htmlSource)).Result;
            
            return document?
                .QuerySelector($"input[name='{AspNetVerificationTokenName}']")?
                .GetAttribute("value") ?? "";
        }


        private static IEnumerable<Category> GetResultCategories(string htmlSource)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(htmlSource)).Result;

            foreach (var Row in document.QuerySelectorAll("tr[data-tid|='category-row']"))
            {
                var id = Row.GetAttribute("data-tid")?.Split("-").Last();
                var name = Row.QuerySelector("td[data-tid='category-name']")?.Text().Trim();
                var description = Row.QuerySelector("td[data-tid='category-description']")?.Text().Trim();

                yield return new Category
                {
                    CategoryId = int.Parse(id ?? "-1"),
                    CategoryName = name ?? "",
                    Description = description,
                    Picture = null,
                };
            }
        }
        private static IEnumerable<string> GetValidationErrorMessages(string htmlSource)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(htmlSource)).Result;

            foreach (var element in document.QuerySelectorAll("span.field-validation-error"))
            {
                yield return element.InnerHtml;
            }
        }
    }
}
