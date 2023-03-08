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
    public class ProfileEditingTest : SeleniumTestsBase
    {
        private readonly (string email, string pass) user = ("email@yandex.ru", "$Password123!");

        public ProfileEditingTest(BrowserTypes browserType)
           : base(browserType)
        { }

        [SetUp]
        public void SetUp()
        {
            userControl.DeleteAllUsers();
            userControl.AddUser(user.email, user.pass);
        }

        [Test]
        public void ShouldChange_UserPassword()
        {
            string newPassword = "$NewPassword123!";

            var mainPage = GetMainPage();
            var managePage = mainPage
                .GoToLoginPage()
                .Login(user.email, user.pass)
                .GoToProfileEditingPage();
            managePage.passwordLink.Click();

            var result = managePage.ChangePassword(user.pass, newPassword);

            result.Should().NotBeNullOrEmpty();
            result.Should().BeEquivalentTo("Your password has been changed.");

            NavigateToIndex();
            var greeting = mainPage
                .LogOut()
                .GoToLoginPage()
                .Login(user.email, newPassword)
                .GetUserNameWithGreeting();

            greeting.Should().NotBeNullOrEmpty();
            greeting.Should().BeEquivalentTo($"Привествуем {user.email}!");
        }

        [Test]
        public void ShouldAdd_PhoneNumber()
        {
            var mainPage = GetMainPage();
            var pageAction = mainPage
                .GoToLoginPage()
                .Login(user.email, user.pass)
                .GoToProfileEditingPage();
            pageAction.profileLink.Click();
            pageAction.phoneNumber.Clear();
            var result = pageAction.ChangePhoneNumber("89990000000");

            result.Should().NotBeNullOrEmpty();
            result.Should().BeEquivalentTo("Your profile has been updated");
        }

        [Test]
        public void ShouldChange_PhoneNumber()
        {
            var mainPage = GetMainPage();
            var pageAction = mainPage
                .GoToLoginPage()
                .Login(user.email, user.pass)
                .GoToProfileEditingPage();
            pageAction.profileLink.Click();
            pageAction.phoneNumber.Clear();
            pageAction.ChangePhoneNumber("89990000000");
            var result = pageAction.ChangePhoneNumber("89990000000");

            result.Should().NotBeNullOrEmpty();
            result.Should().BeEquivalentTo("Your profile has been updated");
        }

       
    }
}
