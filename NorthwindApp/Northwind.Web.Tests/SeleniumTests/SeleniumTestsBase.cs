using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using HtmlElements;
using Northwind.Web.Tests.SeleniumTests.Pages;

namespace Northwind.Web.Tests.SeleniumTests
{
    //[TestFixture(BrowserTypes.Firefox)]
    [TestFixture(BrowserTypes.Edge)]
    //[TestFixture(BrowserTypes.Chrome)]
    public class SeleniumTestsBase
    {
        protected IWebDriver webDriver;
        protected BrowserTypes browserType;
        protected string testFilesPath;
        protected IdentityTestHelper userControl;
        protected IPageObjectFactory pageFactory;


        public SeleniumTestsBase(BrowserTypes browserType)
        {
            pageFactory = new PageObjectFactory();

            this.browserType = browserType;
            testFilesPath = Path.Combine(
                Path.GetDirectoryName(this.GetType().Assembly.Location),
                "TestFiles");
        }

        [SetUp]
        public void SetUp()
        {
            userControl = new IdentityTestHelper();

            switch (browserType)
            {
                case BrowserTypes.Firefox:
                    new DriverManager().SetUpDriver(new FirefoxConfig());
                    webDriver = new FirefoxDriver();
                    break;
                case BrowserTypes.Edge:
                    new DriverManager().SetUpDriver(new EdgeConfig());
                    webDriver = new EdgeDriver();
                    break;
                case BrowserTypes.Chrome:
                    new DriverManager().SetUpDriver(new ChromeConfig());
                    webDriver = new ChromeDriver();
                    break;
            }
        }

        public void NavigateToIndex() => webDriver.Navigate().GoToUrl("http://localhost:5129/");

        public MainPage GetMainPage()
        {
            NavigateToIndex();
            return pageFactory.Create<MainPage>(webDriver);
        }

        [TearDown]
        public void TearDown()
        {
            webDriver?.Dispose();
        }
    }
}