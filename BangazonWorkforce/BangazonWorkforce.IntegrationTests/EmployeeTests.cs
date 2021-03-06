﻿using AngleSharp.Dom.Html;
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
using System.Diagnostics;
using System.Collections.Specialized;

namespace BangazonWorkforce.IntegrationTests
{
    public class EmployeeTests :

        IClassFixture<WebApplicationFactory<BangazonWorkforce.Startup>>
    {
        private readonly HttpClient _client;

        public EmployeeTests(WebApplicationFactory<BangazonWorkforce.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        // Integration Testing for displaying List of employees with department.

        [Fact]
        public async Task Get_IndexReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            string url = "/employee";
            
            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

           
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            IHtmlCollection<IElement> tds = indexPage.QuerySelectorAll("td");
            Assert.Contains(tds, td => td.TextContent.Trim() == "FirstName1");
            Assert.Contains(tds, td => td.TextContent.Trim() == "LastName1");
            Assert.Contains(tds, td => td.TextContent.Trim() == "Accounting");


        }

        [Fact]
        public async Task Post_CreateAddsEmployee()
        {
            // Arrange
            Department department = (await GetAllDepartments()).First();
            string url = "/employee/create";
            HttpResponseMessage createPageResponse = await _client.GetAsync(url);
            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(createPageResponse);

            string newFirstName = "FirstName-" + Guid.NewGuid().ToString();
            string newLastName = "LastName-" + Guid.NewGuid().ToString();
            string isSupervisor = "true";
            string departmentId = department.Id.ToString();
            string departmentName = department.Name;


            // Act
            HttpResponseMessage response = await _client.SendAsync(
                createPage,
                new Dictionary<string, string>
                {
                    {"Employee_FirstName", newFirstName},
                    {"Employee_LastName", newLastName},
                    {"Employee_IsSupervisor", isSupervisor},
                    {"Employee_DepartmentId", departmentId}
                });


            // Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            var lastRow = indexPage.QuerySelector("tbody tr:last-child");

            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newFirstName));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newLastName));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(departmentName));

           
        }

        [Fact]
        public async Task Post_EditWillUpdateEmployee()
        {
            // Arrange
            Employee employee = (await GetAllEmloyees()).Last();
            Department department = (await GetAllDepartments()).Last();

            string url = $"employee/edit/{employee.Id}";
            HttpResponseMessage editPageResponse = await _client.GetAsync(url);
            IHtmlDocument editPage = await HtmlHelpers.GetDocumentAsync(editPageResponse);

            string firstName = StringHelpers.EnsureMaxLength(
                employee.FirstName + Guid.NewGuid().ToString(), 55);
            string lastName = StringHelpers.EnsureMaxLength(
                employee.LastName + Guid.NewGuid().ToString(), 55);
            string isSupervisor = employee.IsSupervisor ? "false" : "true";
            string departmentId = department.Id.ToString();
            string departmentName = department.Name;


            // Act
            HttpResponseMessage response = await _client.SendAsync(
                editPage,
                new Dictionary<string, string>
                {
                    {"Employee_FirstName", firstName},
                    {"Employee_LastName", lastName},
                    {"Employee_IsSupervisor", isSupervisor},
                    {"Employee_DepartmentId", departmentId}
                });


            // Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            var lastRow = indexPage.QuerySelector("tbody tr:last-child");

            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(firstName));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(lastName));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(departmentName));   
        }

        private async Task<List<Employee>> GetAllEmloyees()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionSring))
            {
                IEnumerable<Employee> allEmployees =
                    await conn.QueryAsync<Employee>( @"SELECT Id, FirstName, LastName, 
                                                              IsSupervisor, DepartmentId 
                                                         FROM Employee
                                                     ORDER BY Id");
                return allEmployees.ToList();
            }
        }

        private async Task<List<Department>> GetAllDepartments()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionSring))
            {
                IEnumerable<Department> allDepartments =
                    await conn.QueryAsync<Department>(@"SELECT Id, Name, Budget FROM Department");
                return allDepartments.ToList();
            }
        }

        [Fact]
        public async Task GetAllEmployeeDetails()
        {
            // Arange
            string url = "/employee/details/26";

            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            IHtmlDocument detailPage = await HtmlHelpers.GetDocumentAsync(response);
            IHtmlCollection<IElement> dds = detailPage.QuerySelectorAll("dd");
            Assert.Contains(dds, dd => dd.TextContent.Trim() == "Jeff");
            Assert.Contains(dds, dd => dd.TextContent.Trim() == "Santos");
            Assert.Contains(dds, dd => dd.TextContent.Trim() == "Accounting");
            Assert.Contains(dds, dd => dd.TextContent.Trim() == "Dell");
            Assert.Contains(dds, dd => dd.TextContent.Trim() == "Cool Comp");
            Assert.Contains(dds, dd => dd.TextContent.Trim() == "Eff You");
        }
    }
}
