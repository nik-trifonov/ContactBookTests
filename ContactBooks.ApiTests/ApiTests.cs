using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace ContactBooks.ApiTests
{
    public class ApiTests
    {
        private const string url = "https://contactbook.nakov.repl.co/api/contacts";
        private RestClient client;
        private RestRequest request;
        [SetUp]
        public void Setup()
        {
            this.client = new RestClient();
        }

        [Test]
        public void Test_GetAllClients_CheckFirstClient()
        {
            this.request = new RestRequest(url);

            var response = this.client.Execute(request);
            var contacts = JsonSerializer.Deserialize<List<Contacts>>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(expected: HttpStatusCode.OK));
            Assert.That(contacts.Count, Is.GreaterThan(0));
            Assert.That(contacts[0].firstName, Is.EqualTo("Steve"));
            Assert.That(contacts[0].lastName, Is.EqualTo("Jobs"));
        }

        [Test]
        public void Test_GetAllClients_CheckResult()
        {
            this.request = new RestRequest(url + "/search/{keyword}");
            request.AddUrlSegment("keyword", "albert");

            var response = this.client.Execute(request);
            var contacts = JsonSerializer.Deserialize<List<Contacts>>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(contacts.Count, Is.GreaterThan(0));
            Assert.That(contacts[0].firstName, Is.EqualTo("Albert"));
            Assert.That(contacts[0].lastName, Is.EqualTo("Einstein"));
        }

        [Test]
        public void Test_SearchClients_EmptyResults()
        {
            this.request = new RestRequest(url + "/search/{keyword}");
            request.AddUrlSegment("keyword", "missin462378462378");

            var response = this.client.Execute(request, Method.Get);
            var contacts = JsonSerializer.Deserialize<List<Contacts>>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(contacts.Count, Is.EqualTo(0));
          
        }


        [Test]
        public void Test_CreateContact_EmptyLastName()
        {
            this.request = new RestRequest(url);
            var body = new
            {
                firstName = "Gulia",
                email = "gulia@abv.bg",
                phone = "858559"
            };
            request.AddJsonBody(body);
            var response = this.client.Execute(request, Method.Post);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Is.EqualTo("{\"errMsg\": \"Last name cannot be empty!\"}"));
           

        }

        [Test]
        public void Test_CreateContact_ValidData()
        {
            this.request = new RestRequest(url);
            var body = new
            {
                firstName = "Gulia" + DateTime.Now.Ticks,
                lastName = "GuliaLas" + DateTime.Now.Ticks,
                email = + DateTime.Now.Ticks + "gulia@abv.bg",
                phone = "858559"  + DateTime.Now.Ticks,
            };
            request.AddJsonBody(body);
            var response = this.client.Execute(request, Method.Post);


            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var allContacts = this.client.Execute(request, Method.Get);
            var contacts = JsonSerializer.Deserialize<List<Contacts>>(allContacts.Content);
            var lastContact = contacts.LastIndexOf();
            Assert.That(lastContact.firstName, Is.EqualTo(body.firstName));
            Assert.That(lastContact.lastName, Is.EqualTo(body.lastName));


        }
    }
}