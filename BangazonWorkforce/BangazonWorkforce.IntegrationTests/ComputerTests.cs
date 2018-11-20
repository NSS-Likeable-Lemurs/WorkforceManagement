using AngleSharp.Dom.Html;
using BangazonWorkforce.IntegrationTests.Helpers;
using BangazonWorkforce.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Dapper;
using AngleSharp.Dom;

namespace BangazonWorkforce.IntegrationTests
{
    public class ComputerTests :
        IClassFixture<WebApplicationFactory<BangazonWorkforce.Startup>>
    {
        private readonly HttpClient _client;

        public ComputerTests(WebApplicationFactory<BangazonWorkforce.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        // Integration Testing for displaying List of computers.

        [Fact]
        public async Task Get_IndexReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            string url = "/computer";

            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            IHtmlCollection<IElement> tds = indexPage.QuerySelectorAll("td");
            Assert.Contains(tds, td => td.TextContent.Trim() == "Mac");
            Assert.Contains(tds, td => td.TextContent.Trim() == "Apple");
            Assert.Contains(tds, td => td.TextContent.Trim() == "9/30/2018 10:34:09 PM");
        }

        [Fact]
        public async Task Post_CreateAddsComputer()
        {
            // Arrange
            string url = "/computer/create";
            HttpResponseMessage createPageResponse = await _client.GetAsync(url);
            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(createPageResponse);

            string newMake = StringHelpers.EnsureMaxLength("Make-" + Guid.NewGuid().ToString(), 55);
            string newManufacturer = StringHelpers.EnsureMaxLength("Manufacturer-" + Guid.NewGuid().ToString(), 55);
            string newPurchaseDate = DateTime.Now.ToString();



            // Act
            HttpResponseMessage response = await _client.SendAsync(
                createPage,
                new Dictionary<string, string>
                {
                    {"Make", newMake},
                    {"Manufacturer", newManufacturer},
                    {"PurchaseDate", newPurchaseDate}
                });


            // Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            var lastRow = indexPage.QuerySelector("tbody tr:last-child");

            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newMake));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newManufacturer));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newPurchaseDate));
        }






























    }
}
