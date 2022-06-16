using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;

namespace ContactBook.WebDriverTest
{
    public class UITests
    {
        private const string url = "http://contactbook.nakov.repl.co";
        private WebDriver driver;


        [SetUp]
        public void OpenBrowser()
        {
            this.driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }
        [TearDown]
        public void CloseBrowser()
        {
            this.driver.Quit();
          
        }

        [Test]
        public void Test_ListContacts_CheckFirstContact()
        {
            driver.Navigate().GoToUrl(url);
            var contactsLink = driver.FindElement(By.LinkText("Contacts"));

            contactsLink.Click();

            var firstName = driver.FindElement(By.CssSelector("#contact1 > tbody > tr.fname > td")).Text;
            var lastName = driver.FindElement(By.CssSelector("#contact1 > tbody > tr.lname > td")).Text;

            Assert.That(firstName, Is.EqualTo("Steve"));
            Assert.That(lastName, Is.EqualTo("Jobs"));
        }

        [Test]
        public void Test_SearchContacts_CheckFirstResult()
        {
            driver.Navigate().GoToUrl(url);
            driver.FindElement(By.LinkText("Search")).Click();

            var searchField = driver.FindElement(By.Id("keyword"));
            searchField.SendKeys("Albert");
            driver.FindElement(By.Id("search")).Click();


            var firstName = driver.FindElement(By.CssSelector("tr.fname > td")).Text;
            var lastName = driver.FindElement(By.CssSelector("tr.lname > td")).Text;

            Assert.That(firstName, Is.EqualTo("Albert"));
            Assert.That(lastName, Is.EqualTo("Einstein"));
        }

        [Test]
        public void Test_SearchContacts_EmptyResult()
        {
            driver.Navigate().GoToUrl(url);
            driver.FindElement(By.LinkText("Search")).Click();

            var searchField = driver.FindElement(By.Id("keyword"));
            searchField.SendKeys("invalid2635");
            driver.FindElement(By.Id("search")).Click();

            var resultLabel = driver.FindElement(By.Id("searchResult")).Text;
            Assert.That(resultLabel, Is.EqualTo("No contacts found"));  
        }

        [Test]
        public void Test_SearchContacts_InvalidData()
        {
            driver.Navigate().GoToUrl(url);
            driver.FindElement(By.LinkText("Create")).Click();

            var firstName = driver.FindElement(By.Id("firstName"));
            firstName.SendKeys("Gulia");
            var buttonCreate = driver.FindElement(By.Id("create"));
            buttonCreate.Click();

            var errorMsg = driver.FindElement(By.CssSelector("div.err")).Text;
            Assert.That(errorMsg, Is.EqualTo("Error: Last name cannot be empty!");
        }

        [Test]
        public void Test_SearchContacts_ValidData()
        {
            driver.Navigate().GoToUrl(url);
            driver.FindElement(By.LinkText("Create")).Click();

            var firstName = "FirstName" + DateTime.Now.Ticks;
            var lastName = "LastName" + DateTime.Now.Ticks;
            var email = DateTime.Now.Ticks + "gulia@abv.bg";
            var phone ="12345";

            driver.FindElement(By.Id("firstName")).SendKeys(firstName);
            driver.FindElement(By.Id("lastName")).SendKeys(lastName);
            driver.FindElement(By.Id("email")).SendKeys(email);
            driver.FindElement(By.Id("phone")).SendKeys(phone);

            var buttonCreate = driver.FindElement(By.Id("create"));
            buttonCreate.Click();

            var allContacts = driver.FindElement(By.CssSelector("table.contact-entry"));
            var lastContact = allContacts.Last();

            var firstNameLabel = lastContact.FindElement(By.CssSelector("tr.fname > td")).Text;
            var lastNameLabel = lastContact.FindElement(By.CssSelector("tr.lname > td")).Text;

            Assert.That(firstNameLabel, Is.EqualTo(firstName));
            Assert.That(lastNameLabel, Is.EqualTo(lastName));



        }
    }
}