using HtmlElements.Elements;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace Northwind.Web.Tests.SeleniumTests.Pages
{
    public class MainPage : HtmlPage
    {
        [FindsBy(How = How.CssSelector, Using = "a[href*='Categories'].nav-link")]
        private HtmlLink categoriesLink;

        [FindsBy(How = How.CssSelector, Using = "a[href*='Identity/Account/Register'].nav-link")]
        private HtmlLink registerLink;

        [FindsBy(How = How.CssSelector, Using = "a[href*='Identity/Account/Login'].nav-link")]
        private HtmlLink loginLink;

        [FindsBy(How = How.CssSelector, Using = "a[href*='Identity/Account/Manage'].nav-link")]
        private HtmlLink manageLink;

        [FindsBy(How = How.CssSelector, Using = "form.form-inline")]
        private HtmlForm logOutForm;

        public MainPage(ISearchContext webDriverOrWrapper) : base(webDriverOrWrapper)
        {
        }

        public CategoryListPage GoToCategoriesListPage()
        {
            categoriesLink.Click();
            return PageObjectFactory.Create<CategoryListPage>(this);
        }

        public RegistrationPage GoToRegistrationPage()
        {
            registerLink.Click();
            return PageObjectFactory.Create<RegistrationPage>(this);
        }

        public LoginPage GoToLoginPage()
        {
            loginLink.Click();
            return PageObjectFactory.Create<LoginPage>(this);
        }

        public ProfileEditingPage GoToProfileEditingPage()
        {
            manageLink.Click();
            return PageObjectFactory.Create<ProfileEditingPage>(this);
        }

        public MainPage LogOut()
        {
            logOutForm.Submit();
            return this;
        }

        public string GetUserNameWithGreeting() => manageLink.Text;
    }
}

