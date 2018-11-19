using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModels;

namespace BangazonWorkforce.Controllers

/**
* Class: DepartmentController
* Purpose: Define all methods that interract with the Department table in the database, and routes to create
        routes to create new users.
* Author: Helen Chalmers and Team 
* Methods:
*   DepartmentController(BangazonDeltaContext) - Constructor that gives access to models on creation
*   Create() - returns the Department/Create.cshtml view
*   Create(Department department) - Method called upon the submit button on the new Department form, to create and add a new Department
                    to the database.
        Department department - the properties of the department collected through the form that is added to the database
Get() - Returns all Departments from the Department table in the database.

*/
{
    public class DepartmentController : Controller
    {
        private IConfiguration _config;
        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public DepartmentController(IConfiguration config)
        {
            _config = config;
        }
        /* Purpose: Get() - Returns all Departments from the Department table in the database.  
         * Author: Helen Chalmers
         */
        public async Task<IActionResult> Index()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT d.Id,d.Name, d.Budget, COUNT(e.DepartmentId) TotalEmployees 
                                FROM Department d 
                                LEFT JOIN Employee e on d.Id = e.DepartmentId 
                                GROUP BY d.Id,d.Name, d.Budget;";               
                    IEnumerable<Department> departments = await conn.QueryAsync<Department>(sql);

                return View(departments);
            }
        }

        /**
       *Purpose: Define Detail method that interract with the Department table to show detail of Department
                 with the employees assign to that department.
       *Author: Priynaka Garg
       *Method: DepartmentDetail([FromRoute]int id) - When a user on Department page clicks on any Detail hyperlinked Department then page load the detail of that Department",
    */

        public async Task<IActionResult> Details(int id) 
        {
           
            string sql = $@"SELECT d.Id, 
                             d.Name,  
                             e.Id,
                           e.FirstName, 
                           e.LastName,
                           e.DepartmentId
                         FROM Department d 
                LEFT JOIN Employee e on d.Id = e.DepartmentId
                    WHERE d.Id = {id}";

            using (IDbConnection conn = Connection)
            {

              DepartmentDetailViewModel dept = new DepartmentDetailViewModel();
                IEnumerable<Department> deptquery = await conn.QueryAsync<Department, Employee, Department>(sql,
                   (department, employee) =>
                   {

                       dept.Id = department.Id;
                       dept.Name = department.Name;


                       dept.Employees.Add(employee);
                       return department;
                   }
                   );

                return View(dept);
            }
        }



        // GET: Department/Create
        public IActionResult Create()
        {
            return View();
        }

        //  Purpose: POST: Department/Create
        //   Author: Helen Chalmers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Budget")] Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }

            using (IDbConnection conn = Connection)
            {
                string sql = $@"INSERT INTO Department (Name, Budget) 
                                     VALUES ('{department.Name}', {department.Budget});";

                await conn.ExecuteAsync(sql);
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Department/Edit/5
        // Author: Team
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Department department = await GetById(id.Value);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Department/Edit/5
        // Author: Team
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(department);
            }

            using (IDbConnection conn = Connection)
            {
                string sql = $@"UPDATE Department 
                                   SET Name = '{department.Name}', 
                                       Budget = {department.Id}
                                 WHERE id = {id}";

                await conn.ExecuteAsync(sql);
                return RedirectToAction(nameof(Index));
            }
        }


        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Department department = await GetById(id.Value);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Department/Delete/5
        // Author: Team 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"DELETE FROM Department WHERE id = {id}";
                int rowsDeleted = await conn.ExecuteAsync(sql);
                
                if (rowsDeleted > 0)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
        }


        private async Task<Department> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT Id, Name, Budget 
                                  FROM Department
                                 WHERE id = {id}";

                IEnumerable<Department> departments = await conn.QueryAsync<Department>(sql);
                return departments.SingleOrDefault();
            }
        }
    }
}