using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Dapper;


/**
 * Class: EmployeeController
 * Purpose: Define all methods that interract with the Employee table in the database, and routes to
            list all employees with department name.
 * Author: Team-Likeable Lemurs
 * Methods:
 * EmployeeController(BangazonWorkForce) - Constructor that gives access to models on creation
 */
namespace BangazonWorkforce.Controllers
{
    public class EmployeeController : Controller
    {
        private IConfiguration _config;
        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public EmployeeController(IConfiguration config)
        {
            _config = config;
        }


   /**
       *Purpose: Define Index method that interract with the Employee table in the database, and routes to
            list all employees with department name.
       *Author: Priyanka Garg
       *Methods: Index() - When the page loads user be able to see list of all employees and department name from the context  ",
       */


        public async Task<IActionResult> Index()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = @"SELECT e.Id, 
                                      e.FirstName,
                                      e.LastName, 
                                      e.IsSupervisor,
                                      e.DepartmentId,
                                      d.Id,
                                      d.Name,
                                      d.Budget,
                                      c.Id,
                                      c.PurchaseDate,
                                      c.DecomissionDate,
                                      c.Make,
                                      c.Manufacturer
                                 FROM Employee e 
                                 LEFT JOIN Department d on e.DepartmentId = d.Id
                                 LEFT JOIN ComputerEmployee ce on e.Id = ce.EmployeeId
                                 LEFT JOIN Computer c on ce.ComputerId = c.ID
                             WHERE UnassignDate IS NULL
                             ORDER BY e.Id";
                IEnumerable<Employee> employees = await conn.QueryAsync<Employee, Department, Computer, Employee>(
                    sql,
                    (employee, department, computer) => {
                        employee.Department = department;
                        employee.Computer = computer;
                        return employee;
                    });

                return View(employees);
            }
        }
        /**
       *Purpose: Define Detail method that interract with the Employee table to show detail of employee
                 with department name.
       *Author: Kelly Cook
       *Method: EmployeeDetail([FromRoute]int id) - When a user on Employee page clicks on any Detail hyperlinked Employee then page load the detail of that Employee",
    */
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Employee employee = await GetById(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }
      /**
       *Purpose: Define Create method that Insert the data for Employee table in the database.
       *Author: Streator ward
       *Method: Create() - When User clicks Create link on Employee Page then a page opens with blank form to creat new employee.",
    */
          public async Task<IActionResult> Create()
        {
            List<Department> allDepartments = await GetAllDepartments();
            EmployeeAddEditViewModel viewmodel = new EmployeeAddEditViewModel
            {
                AllDepartments = allDepartments
            };
            return View(viewmodel);
        }

        // POST: Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeAddEditViewModel viewmodel)
        {
            if (!ModelState.IsValid)
            {
                List<Department> allDepartments = await GetAllDepartments();
                viewmodel.AllDepartments = allDepartments;
                return View(viewmodel);
            }

            Employee employee = viewmodel.Employee;

            using (IDbConnection conn = Connection)
            {
                string sql = $@"INSERT INTO Employee (
                                    FirstName, LastName, IsSupervisor, DepartmentId
                                ) VALUES (
                                    '{employee.FirstName}', '{employee.LastName}',
                                    {(employee.IsSupervisor ? 1 : 0)}, {employee.DepartmentId}
                                );";

                await conn.ExecuteAsync(sql);
                return RedirectToAction(nameof(Index));
            }
        }
        /**
            *Purpose: Define Edit method that interact with the Employee table in the database, and Edit the data for existing employee.
            *Author: Aron Keen
            *Method: Edit([FromRoute]int id) - When a user click the Edit Link on Employee page then a a page open with a form to edit existing employee.
         */

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Computer> allComputers = await GetOpenComputers();
            List<Department> allDepartments = await GetAllDepartments();
            Employee employee = await GetById(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            EmployeeAddEditViewModel viewmodel = new EmployeeAddEditViewModel
            {
                Employee = employee,
                AllDepartments = allDepartments,
                AvailableComputers = allComputers
            };

            return View(viewmodel);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeAddEditViewModel viewmodel)
        {

            Employee employee = viewmodel.Employee;

            using (IDbConnection conn = Connection)
            {
                string sql = $@"UPDATE Employee 
                                   SET LastName = '{employee.LastName}', 
                                       DepartmentId = {employee.DepartmentId}
                                 WHERE id = {id};";

                string unassignComputerSql = $@"UPDATE ComputerEmployee
                                                   SET UnassignDate = '{DateTime.Now}'
                                                WHERE id = {id}";

                string assignComputerSql = $@"INSERT INTO ComputerEmployee 
                                   (EmployeeId, ComputerId, AssignDate) 
                                 VALUES ({id}, {viewmodel.Employee.Computer.Id}, '{DateTime.Now}');";

                sql = sql + assignComputerSql + unassignComputerSql;
                await conn.ExecuteAsync(sql);
                return RedirectToAction("Index");
            }
        }
         /**
            *Purpose: Define Delete method to delete the particular Employee in the database,
            *Author: 
            *Method: Delete([FromRoute]int id) - When a user click the delete Link on Employee Page then a page open with message "Are you sure you want to delete this employee".
    */
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Employee employee = await GetById(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"DELETE FROM Employee WHERE id = {id}";
                await conn.ExecuteAsync(sql);
                return RedirectToAction(nameof(Index));
            }
        }


        private async Task<Employee> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT e.Id, 
                                       e.FirstName,
                                       e.LastName, 
                                       e.IsSupervisor,
                                       e.DepartmentId,
                                       d.Id,
                                       d.Name,
                                       d.Budget
                                  FROM Employee e JOIN Department d on e.DepartmentId = d.Id
                                 WHERE e.id = {id}";
                IEnumerable<Employee> employees = await conn.QueryAsync<Employee, Department, Employee>(
                    sql,
                    (employee, department) => {
                        employee.Department = department;
                        return employee;
                    });

                return employees.SingleOrDefault();
            }
        }

        private async Task<List<Department>> GetAllDepartments()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT Id, Name, Budget FROM Department";

                IEnumerable<Department> departments = await conn.QueryAsync<Department>(sql);
                return departments.ToList();
            }
        }

        private async Task<List<Computer>> GetOpenComputers()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT c.Id, 
                                       c.PurchaseDate, 
                                       c.DecomissionDate, 
                                       c.Make, 
                                       c.Manufacturer 
                               FROM Computer c 
                               LEFT JOIN ComputerEmployee ce on ce.ComputerId = c.Id
                               WHERE ce.Id IS NULL
                               AND DecomissionDate IS NULL";

                IEnumerable<Computer> computers = await conn.QueryAsync<Computer>(sql);
                return computers.ToList();
            }
        }
    }
}