using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
    public class ComputerController : Controller
    {
        private readonly IConfiguration _config;

        public ComputerController(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Computer
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string sql = $@"SELECT
                            c.Id,
                            c.Manufacturer,
                            c.Make,
                            c.PurchaseDate
                            FROM Computer c;";

            using (IDbConnection conn = Connection)
            {
                List<Computer> computer = (await conn.QueryAsync<Computer>(sql)).ToList();
                return View(computer);
            }
        }

        // GET: Computer/Details/5
        public async Task<IActionResult> Details(int id)
        {
            string sql = $@"SELECT
                            c.Id,
                            c.Manufacturer,
                            c.Make,
                            c.PurchaseDate
                            FROM Computer c
                            WHERE c.Id = {id};";

            using (IDbConnection conn = Connection)
            {
                if (CheckComputerDoesNotExist(id))
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var computer = (await conn.QueryAsync<Computer>(sql)).Single();
                    return View(computer);
                }
            }
        }

        private bool CheckComputerDoesNotExist(int id)
        {
            string sql = $"SELECT * FROM Computer c WHERE c.Id = {id};";

            using (IDbConnection conn = Connection)
            {
                var theCount = conn.Query<Computer>(sql).Count();
                return theCount == 0;
            }
        }

        // GET: Computer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Computer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Computer newComputer)
        {

            if (!(ModelState.IsValid))
            {
                return View(newComputer);
            }
            else
            {
                string sql = $@"
				INSERT INTO Computer
					(Manufacturer, Make, PurchaseDate)
				VALUES
					('{newComputer.Manufacturer}', '{newComputer.Make}', '{newComputer.PurchaseDate}')
				";

                using (IDbConnection conn = Connection)
                {
                    int rowsAffected = await conn.ExecuteAsync(sql);
                    bool createdSuccessfully = rowsAffected > 0;

                    if (createdSuccessfully)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw new Exception("No rows affected; record was not added to database");
                    }
                }
            }


        }

        // GET: Computer/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Computer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        /* GET: Computer/Delete/5
		 */
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string sql = $@"
			SELECT
				c.Id,
				c.Manufacturer,
				c.Make,
				c.PurchaseDate
			FROM Computer c
			WHERE c.Id = {id}
			";

            using (IDbConnection conn = Connection)
            {
                if (ComputerHasBeenAssigned(id))
                {
                    return View("NoDelete");
                }
                else
                {
                    Computer computer = (await conn.QueryAsync<Computer>(sql)).Single();
                    return View("Delete", computer);
                }
            }
        }

        /*
		 This method checks if a computer has already been assigned to an employee. It is used by the Delete and Delete methods.
		 */
        private bool ComputerHasBeenAssigned(int? id)
        {
            string sql = $@"
			SELECT 
				e.Id,
				e.FirstName,
				e.LastName,
				e.DepartmentId
			FROM Employee e
			JOIN ComputerEmployee ec ON e.Id = ec.EmployeeId
			JOIN Computer c ON ec.ComputerId = c.Id
			WHERE c.Id = {id}
			";

            using (IDbConnection conn = Connection)
            {
                int numAssignments = (conn.Query<Employee>(sql)).Count();
                return numAssignments > 0;
            }
        }

        /* 
		 POST: Computer/Delete/5
		*/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (ComputerHasBeenAssigned(id))
            {
                throw new Exception("You cannot delete a computer that has been assigned to an employee");
            }

            string sql = $@"
				DELETE FROM Computer WHERE Id = {id};
				";

            using (IDbConnection conn = Connection)
            {
                int rowsAffected = await conn.ExecuteAsync(sql);
                bool deleteSuccessful = rowsAffected > 0;

                if (deleteSuccessful)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    throw new Exception("No rows affected");
                }
            }
        }


    }
}