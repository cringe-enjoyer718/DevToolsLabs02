using HtmlElements.Elements;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Web.Tests.SeleniumTests.Pages
{
    public class LoginPage : HtmlPage
    {
        [FindsBy(How = How.Id, Using = "Input_Email")]
        private HtmlInput emailInput;

        [FindsBy(How = How.Id, Using = "Input_Password")]
        private HtmlInput passwordInput;

        [FindsBy(How = How.Id, Using = "account")]
        private HtmlForm loginForm;

        [FindsBy(How = How.CssSelector, Using = "div.text-danger.validation-summary-errors")]
        private HtmlElement errorElement;

        public const string LoginError = "Invalid login attempt.";

        public LoginPage(ISearchContext webDriverOrWrapper) : base(webDriverOrWrapper)
        {
        }

        public string Error => errorElement.Text;

        public string EmailInput
        {
            get => emailInput.Value;
            set => emailInput.SendKeys(value);
        }

        public string PasswordInput
        {
            get => passwordInput.Value;
            set => passwordInput.SendKeys(value);
        }

        public MainPage SubmitForm()
        {
            loginForm.Submit();
            return PageObjectFactory.Create<MainPage>(this);
        }

        public MainPage Login(string email, string pass)
        {
            EmailInput = email;
            PasswordInput = pass;
            return SubmitForm();
        }

    }
}
