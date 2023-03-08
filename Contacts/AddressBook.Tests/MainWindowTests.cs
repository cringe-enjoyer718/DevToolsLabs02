using AddressBook.Tests.Windows;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using FluentAssertions;
using Microsoft.Communications.Contacts;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;


namespace AddressBook.Tests
{
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        ContactManager contactManager;
        private AutomationBase automationBase;
        private Application App;

        Person person = new Person
        {
            Name = "Ivan Ivanov",
            Phone = "89225458899",
            Street = "Studencheskaya",
            City = "Izhevsk",
        };


        [SetUp]
        public void Setup()
        {
            contactManager = new ContactManager();
            ClearContacts();

            automationBase = new UIA3Automation();
            App = FlaUI.Core.Application
                .Launch(@"..\..\..\..\AddressBook\AddressBook.exe");
        }

        [TearDown]
        public void Clear()
        {
            ClearContacts();
            App?.Close();
            automationBase?.Dispose();;
        }

        [Test]
        public void ReadContactList()
        {
            var contacts = GenerateContacts(10).ToList();
            var mainWindow = App.GetMainWindow(automationBase).As<MainWindow>();

            var contactNames = mainWindow.Contacts.Select(c => c.ContactName);
            var expectedNames = contacts.Select(c => c.Names.First().FormattedName);

            contactNames.Should().BeEquivalentTo(expectedNames);
        }

        [Test]
        public void OpenFirstContact()
        {
            var contacts = GenerateContacts(10).ToList();
            var mainWindow = App.GetMainWindow(automationBase).As<MainWindow>();

            var contactWindow = mainWindow.Contacts.First().OpenContactWindow();
            contactWindow.Close();
        }

        private void ClearContacts()
        {
            var contacts = contactManager.GetContactCollection();
            foreach (var contact in contacts)
            {
                contactManager.Remove(contact.Id);
            }
        }

        private IEnumerable<Contact> GenerateContacts(int count)
        {
            for (uint i = 0; i < count; i++)
            {
                var person = new Bogus.Person();

                var contact = new Contact();
                contact.Names.Add(new Name(person.FirstName, "", person.LastName,
                    NameCatenationOrder.GivenFamily));

                contactManager.AddContact(contact);

                yield return contact;
            }

        }

        [Test]
        public void ShouldCreateNewContact()
        {

            var mainWindow = App.GetMainWindow(automationBase).As<MainWindow>();
            var contactWindow = mainWindow.OpenNewContactWindow().As<ContactWindow>();

            var nameTab = contactWindow.OpenNameTab();
            nameTab.FormattedName.Text = person.Name;

            var phoneTab = contactWindow.OpenPhoneTab();
            phoneTab.HomePhone.Text = person.Phone;

            var locationTab = contactWindow.OpenLocationsTab();
            locationTab.City.Text = person.City;
            locationTab.Street.Text = person.Street;

            contactWindow.SaveChanges.Click();

            var contacts = contactManager.GetContactCollection();
            contacts.Should().HaveCount(1);
            var contact = contacts.First();

            contact.Names.First().FormattedName.Should().Be(person.Name);
            contact.PhoneNumbers.Select(p => p.Number).Should().Contain(person.Phone);
            contact.Addresses.Select(a => a.City).Should().Contain(person.City);
            contact.Addresses.Select(a => a.Street).Should().Contain(person.Street);
        }
        [Test]
        public void ShouldEdit_ExistingContact()
        {
            var person = new Person
            {
                Name = "Maxim Maximov",
                Phone = "89120000000",
                Street = "Pushkinskaya",
                City = "Votkinsk"
            };

            GenerateContacts(1).ToList();
            var mainWindow = App.GetMainWindow(automationBase).As<MainWindow>();

            var changingContact = mainWindow.Contacts.First();
            var contactWindow = changingContact.OpenContactWindow();

            var nameTab = contactWindow.OpenNameTab();
            nameTab.FormattedName.Text = person.Name;

            var phoneTab = contactWindow.OpenPhoneTab();
            phoneTab.HomePhone.Text = person.Phone;

            var locationTab = contactWindow.OpenLocationsTab();

            locationTab.City.Text = person.City;
            locationTab.Street.Text = person.Street;

            contactWindow.SaveChanges.Click();

            var contacts = contactManager.GetContactCollection();
            var contact = contacts.First();

            contact.Names.First().FormattedName.Should().Be(person.Name);
            contact.PhoneNumbers.Select(p => p.Number)
                .Should().Contain(person.Phone);
            contact.Addresses.Select(a => a.City).Should().Contain(person.City);
            contact.Addresses.Select(a => a.Street).Should().Contain(person.Street);
        }

        [Test]
        public void ShouldDelete_ExistingContact()
        {
            var contacts = GenerateContacts(10).ToList();
            var mainWindow = App.GetMainWindow(automationBase).As<MainWindow>();

            var deletingContact = contacts.First();

            mainWindow.Contacts.Where(c => c.ContactName == deletingContact.Names.First().FormattedName).First().Select();
            mainWindow.Delete.Click();

            var exceptedContactsNames = contacts.Skip(1).Select(c => c.Names.First()).
                OrderBy(x => x.FormattedName).ToList();

            var actualContactsNames = contactManager.
                GetContactCollection().
                Select(c => c.Names.First()).OrderBy(x => x.FormattedName).ToList();

            exceptedContactsNames.Should().BeEquivalentTo(actualContactsNames);
        }

        [Test]
        public void ShouldSearch_ExistingContact()
        {
            var contacts = GenerateContacts(10).ToList();
            var mainWindow = App.GetMainWindow(automationBase).As<MainWindow>();

            var searchingContactName = contacts[2].Names.First().FormattedName;

            mainWindow.SearchTextBox.Focus();
            mainWindow.SearchTextBox.Text = searchingContactName;

            Retry.WhileTrue(() => mainWindow.Contacts.Count() > 1);
            var actualContactsName = mainWindow.Contacts.First().ContactName;

            mainWindow.Contacts.Should().HaveCount(1);
            searchingContactName.Should().Be(actualContactsName);
        }

    }
}