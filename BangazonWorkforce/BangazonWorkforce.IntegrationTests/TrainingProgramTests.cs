using AngleSharp.Dom.Html;
using BangazonWorkforce.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BangazonWorkforce.IntegrationTests
{
    public class TrainingProgramTests :
        IClassFixture<WebApplicationFactory<BangazonWorkforce.Startup>>
    {
        private readonly HttpClient _client;

        public TrainingProgramTests(WebApplicationFactory<BangazonWorkforce.Startup> factory)
        {
            _client = factory.CreateClient();
        }
        // Author: Helen Chalmers
        // Purpose: Integration Testing for getting the List of TrainingPrograms.
        [Fact]
        public async Task Get_IndexReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            string url = "/trainingprogram";

            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
        //Author: Helen Chalmers
        //Purpose:  Integration Testing for posting a new TrainingProgram to the database.  

        [Fact]
        public async Task Post_CreateAddsTrainingProgram()
        {
            // Arrange
            string url = "/trainingprogram/create";
            HttpResponseMessage createPageResponse = await _client.GetAsync(url);
            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(createPageResponse);

            string newTPName = "TP25";
            string newTPStartDate = "01-25-2020 02:30 PM";
            string newTPEndDate = "02-25-2020 02:45 PM";
            string newTPMaxAttend = "25";


            // Act
            HttpResponseMessage response = await _client.SendAsync(
                createPage,
                new Dictionary<string, string>
                {
                    {"Name", newTPName},
                    {"StartDate", newTPStartDate},
                    {"EndDate", newTPEndDate },
                    {"MaxAttendees", newTPMaxAttend}
                });


            // Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            Assert.Contains(
                indexPage.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newTPName));
            
        }
    }
}

    

