using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace ContactBook.DesktopClientTest
{
    public class DesktopTest
    {
        private const string AppiumUrl = "http://127.0.0.1:4723/wd/hub";
        private const string ContactsBookUrl = "https://contactbook.nakov.repl.co/api";
        private const string AppLocation = @"C:\Work\Desktop\ContactBook-DesktopClient.exe";
        private WindowsDriver<WindowsElement> driver;
        private AppiumOptions options;


        [SetUp]
        public void StartApp()
        {
            options = new AppiumOptions() { PlatformName = "Windows" };
            options.AddAdditionalCapability("app", AppLocation);

            driver = new WindowsDriver<WindowsElement>(new Uri(AppiumUrl), options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TearDown]
        public void CloseApp()
        {
            driver.Quit();
        }
        [Test]
        public void Test_SearchContact_VerifyFirstREsult()
        {
            var urlField = driver.FindElementByAccessibilityId("textBoxApiUrl");
            urlField.Clear();
            urlField.SendKeys(ContactsBookUrl);

            var buttonConnect = driver.FindElementByAccessibilityId("buttonConnect");
            buttonConnect.Click();

            string windowsName = driver.WindowHandles[0];
            driver.SwitchTo().Window(windowsName);

            var editTextField = driver.FindElementByAccessibilityId("textBoxSearch");
            editTextField.SendKeys("steve");

            var buttonSearch = driver.FindElementByAccessibilityId("buttonSearch");
            buttonSearch.Click();

            Thread.Sleep(2000);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(d =>
            {
                var searchLabel = driver.FindElementByAccessibilityId("labelResult").Text;
                return searchLabel.StartsWith("Contacts found");
            });
            var searchLabel = driver.FindElementByAccessibilityId("labelResult");
            Assert.That(searchLabel, Is.EqualTo("contacts found: 1"));

            var firstName = driver.FindElement(By.XPath("//Edit[@Name=\"FirstName Row 0, Not sorted.\"]"));
            var lastName = driver.FindElement(By.XPath("//Edit[@Name=\"LastName Row 0, Not sorted.\"]"));


            Assert.That(firstName.Text, Is.EqualTo("Steve"));
            Assert.That(firstName.Text, Is.EqualTo("Jobs"));


        }
    }
}