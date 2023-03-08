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
    public class RegistrationPage : HtmlPage
    {
        [FindsBy(How = How.Id, Using = "Input_Email")]
        private HtmlInput emailInput;

        [FindsBy(How = How.Id, Using = "Input_Password")]
        private HtmlInput passwordInput;

        [FindsBy(How = How.Id, Using = "Input_ConfirmPassword")]
        private HtmlInput confirmPasswordInput;

        [FindsBy(How = How.Id, Using = "registerForm")]
        private HtmlForm registerForm;

        [FindsBy(How = How.CssSelector, Using = "span.text-danger.field-validation-error")]
        private HtmlElement errorElement;

        public const string ErrorEmail = "The Email field is not a valid e-mail address.";

        public const string ErrorConfirmPassword = "The password and confirmation password do not match.";

        public const string ErrorPasswordLength = "The Password must be at least 6 and at max 100 characters long.";


        public RegistrationPage(ISearchContext webDriverOrWrapper) : base(webDriverOrWrapper)
        {
        }

        public string ErrorMessage => errorElement.Text;

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

        public string ConfirmPasswordInput
        {
            get => confirmPasswordInput.Value;
            set => confirmPasswordInput.SendKeys(value);
        }

        public MainPage SubmitForm()
        {
            registerForm.Submit();
            return PageObjectFactory.Create<MainPage>(this);
        }
    }
}
