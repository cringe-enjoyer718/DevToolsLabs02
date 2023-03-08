using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.AutomationElements.PatternElements;
using FlaUI.Core.Definitions;
using System.Collections.Generic;
using System.Linq;

namespace AddressBook.Tests.Windows
{
    public class ContactWindow : Window
    {
        public ContactWindow(FrameworkAutomationElementBase frameworkAutomationElement) : base(frameworkAutomationElement)
        {
        }
        private Tab Tab => FindFirstDescendant(cf => cf.ByControlType(ControlType.Tab)).AsTab();

        public Button SaveChanges => FindFirstDescendant(cf => cf.ByName("Save Changes")).AsButton();

        public NameTab OpenNameTab()
        {
            var tabItem = Tab.TabItems.Where(item => item.Name.Contains("Name")).First();
            tabItem.Select();

            return tabItem.As<NameTab>();
        }

        public PhoneTab OpenPhoneTab()
        {
            var tabItem = Tab.TabItems.Where(item => item.Name.Contains("Phone")).First();
            tabItem.Select();

            return tabItem.As<PhoneTab>();
        }

        public LocationsTab OpenLocationsTab()
        {
            var tabItem = Tab.TabItems.Where(item => item.Name.Contains("Locations")).First();
            tabItem.Select();

            return tabItem.As<LocationsTab>();
        }
    }

    public class NameTab : SelectionItemAutomationElement
    {
        public NameTab(FrameworkAutomationElementBase frameworkAutomationElement) : base(frameworkAutomationElement)
        {
            textBoxes = FindFirstDescendant(cf => cf.ByName("Name"))
            .FindAllChildren(cf => cf.ByClassName("TextBox"))
            .Select(e => e.AsTextBox()).ToList();
        }

        private List<TextBox> textBoxes;

        public TextBox FormattedName => textBoxes[0];
    }

    public class PhoneTab : SelectionItemAutomationElement
    {
        public PhoneTab(FrameworkAutomationElementBase frameworkAutomationElement) : base(frameworkAutomationElement)
        {
        }

        public TextBox HomePhone =>
            FindFirstDescendant(cf => cf.ByAutomationId("_homePhoneBox"))
            .AsTextBox();
    }

    public class LocationsTab : SelectionItemAutomationElement
    {
        public LocationsTab(FrameworkAutomationElementBase frameworkAutomationElement) : base(frameworkAutomationElement)
        {
            textBoxes = FindFirstDescendant(cf => cf.ByName("Home Address"))
            .FindAllDescendants(cf => cf.ByClassName("TextBox"))
            .Select(e => e.AsTextBox()).ToList();
        }

        private List<TextBox> textBoxes;

        public TextBox Street => textBoxes[0];

        public TextBox City => textBoxes[1];
    }

}
