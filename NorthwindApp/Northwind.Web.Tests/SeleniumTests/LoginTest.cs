using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using HtmlElements;
using Northwind.Web.Tests.SeleniumTests.Pages;

namespace Northwind.Web.Tests.SeleniumTests
{
    public class LoginTest : SeleniumTestsBase
    {
        private readonly string email = "email@yandex.ru";
        private readonly string pass = "$Password123!";


        public LoginTest(BrowserTypes browserType)
           : base(browserType)
        { }

        [SetUp]
        public void SetUp()
        {
            userControl.DeleteAllUsers();
            userControl.AddUser(email, pass);
        }

        [Test]
        public void ShouldLogIn_ThenLogOut()
        {
            var mainPage = GetMainPage();
            var loginPage = mainPage.GoToLoginPage();
            loginPage.EmailInput = email;
            loginPage.PasswordInput = pass;

            mainPage = loginPage.SubmitForm();

            new Uri(webDriver.Url).AbsolutePath.Should().BeEquivalentTo("/");
            mainPage.Should().NotBeNull();
            mainPage.GetUserNameWithGreeting().Should().BeEquivalentTo($"Привествуем {email}!");

            mainPage.LogOut();
            mainPage.Should().NotBeNull();

            Action action = () => mainPage.GetUserNameWithGreeting();
            action.Should().Throw<NoSuchElementException>();
        }

        [Test]
        public void Should_NotLogIn_WithInvalidInput()
        {
            var mainPage = GetMainPage();
            var loginPage = mainPage.GoToLoginPage();

            loginPage.EmailInput = email;
            loginPage.PasswordInput = "rKdjf2fls!$lj";
            loginPage.SubmitForm();

            new Uri(webDriver.Url).AbsolutePath.Should().NotBeEquivalentTo("/");
            loginPage.Error.Should().BeEquivalentTo(LoginPage.LoginError);
        }
    }
}
