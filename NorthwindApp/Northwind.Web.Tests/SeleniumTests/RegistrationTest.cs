using FluentAssertions;
using HtmlElements;
using Northwind.Web.Tests.SeleniumTests.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Web.Tests.SeleniumTests
{
    public class RegistrationTest : SeleniumTestsBase
    {
        public RegistrationTest(BrowserTypes browserType)
           : base(browserType)
        { }

        [SetUp]
        public void SetUp()
        {
            userControl.DeleteAllUsers();
        }

        [Test]
        public void ShouldRegisterUser_ThenLogOut()
        {
            string email = "email@yandex.ru";
            string password = "$Password123!";

            var mainPage = GetMainPage();
            var registrationPage = mainPage.GoToRegistrationPage();

            registrationPage.EmailInput = email;
            registrationPage.PasswordInput = password;
            registrationPage.ConfirmPasswordInput = password;

            mainPage = registrationPage.SubmitForm();

            new Uri(webDriver.Url).AbsolutePath.Should().BeEquivalentTo("/");
            mainPage.Should().NotBeNull();
            mainPage.GetUserNameWithGreeting().Should().BeEquivalentTo($"Привествуем {email}!");


            mainPage.LogOut();
            mainPage.Should().NotBeNull();

            Action action = () => mainPage.GetUserNameWithGreeting();
            action.Should().Throw<NoSuchElementException>();

            userControl.GetUsers().Should().HaveCount(1);
        }

        [Test]
        public void Should_NotRegister_WithInvalid_Email()
        {
            string email = "privet";
            string password = "Qwerty12345.";

            var mainPage = GetMainPage();
            var registrationPage = mainPage.GoToRegistrationPage();

            registrationPage.EmailInput = email;
            registrationPage.PasswordInput = password;
            registrationPage.ConfirmPasswordInput = password;
            registrationPage.SubmitForm();

            registrationPage.ErrorMessage.Should().BeEquivalentTo(RegistrationPage.ErrorEmail);

            new Uri(webDriver.Url).AbsolutePath.Should().BeEquivalentTo("/Identity/Account/Register");
            userControl.GetUsers().Should().HaveCount(0);
        }


        [Test]
        public void Should_NotRegister_WithInvalid_PasswordConfirm()
        {
            string email = "email@yandex.ru";
            string password = "$Password123!";
            string confirmPassword = "Password123!";

            var mainPage = GetMainPage();
            var registrationPage = mainPage.GoToRegistrationPage();

            registrationPage.EmailInput = email;
            registrationPage.PasswordInput = password;
            registrationPage.ConfirmPasswordInput = confirmPassword;
            registrationPage.SubmitForm();

            registrationPage.ErrorMessage.Should().BeEquivalentTo(RegistrationPage.ErrorConfirmPassword);

            new Uri(webDriver.Url).AbsolutePath.Should().BeEquivalentTo("/Identity/Account/Register");
            userControl.GetUsers().Should().HaveCount(0);
        }


        [Test]
        public void Should_NotRegister_WithInvalid_Password()
        {
            string email = "email@yandex.ru";
            string password = "111";

            var mainPage = GetMainPage();
            var registrationPage = mainPage.GoToRegistrationPage();

            registrationPage.EmailInput = email;
            registrationPage.PasswordInput = password;
            registrationPage.ConfirmPasswordInput = password;

            registrationPage.SubmitForm();
            registrationPage.ErrorMessage.Should().BeEquivalentTo(RegistrationPage.ErrorPasswordLength);

            new Uri(webDriver.Url).AbsolutePath.Should().BeEquivalentTo("/Identity/Account/Register");
            userControl.GetUsers().Should().HaveCount(0);
        }

    }
}
