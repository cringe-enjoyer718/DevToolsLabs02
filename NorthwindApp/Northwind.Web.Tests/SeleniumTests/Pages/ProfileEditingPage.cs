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
    public class ProfileEditingPage : HtmlPage
    {
        [FindsBy(How = How.Id, Using = "profile")]
        public HtmlLink profileLink;

        [FindsBy(How = How.Id, Using = "Input_PhoneNumber")]
        public HtmlInput phoneNumber;

        [FindsBy(How = How.Id, Using = "profile-form")]
        public HtmlForm profileForm;

        [FindsBy(How = How.Id, Using = "email")]
        public HtmlLink emailLink;

        [FindsBy(How = How.Id, Using = "Input_NewEmail")]
        public HtmlInput newEmail;

        [FindsBy(How = How.Id, Using = "email-form")]
        public HtmlForm emailForm;

        [FindsBy(How = How.Id, Using = "change-password")]
        public HtmlLink passwordLink;

        [FindsBy(How = How.Id, Using = "Input_OldPassword")]
        public HtmlInput oldPassword;

        [FindsBy(How = How.Id, Using = "Input_NewPassword")]
        public HtmlInput newPassword;

        [FindsBy(How = How.Id, Using = "Input_ConfirmPassword")]
        public HtmlInput newPasswordConfirm;

        [FindsBy(How = How.Id, Using = "change-password-form")]
        public HtmlForm passwordForm;

        [FindsBy(How = How.Id, Using = "personal-data")]
        public HtmlLink personalDataLink;

        [FindsBy(How = How.Id, Using = "delete")]
        public HtmlLink deleteProfile;

        [FindsBy(How = How.Id, Using = "Input_Password")]
        public HtmlInput password;

        [FindsBy(How = How.Id, Using = "delete-user")]
        public HtmlForm deleteUserForm;

        [FindsBy(How = How.CssSelector, Using = "div.alert.alert-success.alert-dismissible")]
        public HtmlElement alertSuccess;

        public ProfileEditingPage(ISearchContext webDriverOrWrapper) : base(webDriverOrWrapper)
        {
        }

        public string ChangePhoneNumber(string phoneNumber)
        {
            this.phoneNumber.SendKeys(phoneNumber);
            profileForm.Submit();

            return alertSuccess.Text;
        }

        public string ChangePassword(string oldPass, string newPass)
        {
            oldPassword.SendKeys(oldPass);
            newPassword.SendKeys(newPass);
            newPasswordConfirm.SendKeys(newPass);
            passwordForm.Submit();

            return alertSuccess.Text;
        }

        public MainPage DeleteProfile(string pass)
        {
            password.SendKeys(pass);
            deleteUserForm.Submit();

            return PageObjectFactory.Create<MainPage>(this);
        }
    }
}
