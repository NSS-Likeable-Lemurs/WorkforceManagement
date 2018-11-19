using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using BangazonWorkforce.Models;


namespace BangazonWorkforce.Controllers

/**
* Class: TrainingProgramController
* Purpose: Define all methods that interract with the TrainingProgram table in the database, and routes to create
    routes to create new users.
* Author: Helen Chalmers  
* Methods:
*   TrainingProgramController(BangazonDeltaContext) - Constructor that gives access to models on creation
*   
*/
{
    public class TrainingProgramController : Controller

    {
        private IConfiguration _config;
        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public TrainingProgramController(IConfiguration config)
        {
            _config = config;
        }
        /* Purpose: Get() - Returns all TrainingPrograms from the TrainingProgram table in the database that have not occured yet.  
         * Author: Helen Chalmers
         */

        // GET: TrainingProgram
        public async Task<IActionResult> Index()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT tp.Name
		                        FROM TrainingProgram tp
                                WHERE GETDATE() < StartDate;";
                IEnumerable<TrainingProgram> trainingprograms = await conn.QueryAsync<TrainingProgram>(sql);

                return View(trainingprograms);
            }
        }

        // GET: TrainingProgram/Details/5
        public ActionResult Details(int id)
        { 
            return View();
        }

        // GET: TrainingProgram/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingProgram/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, StartDate, EndDate, MaxAttendees")] TrainingProgram trainingprogram)
        {
            if (!ModelState.IsValid)
            {
                return View(trainingprogram);
            }

            using (IDbConnection conn = Connection)
            {
                string sql = $@"INSERT INTO TrainingProgram
                                ([Name], StartDate, EndDate, MaxAttendees)
                                Values('{trainingprogram.Name}', '{trainingprogram.StartDate}', '{trainingprogram.EndDate}', {trainingprogram.MaxAttendees});";

                await conn.ExecuteAsync(sql);
                return RedirectToAction(nameof(Index));
            }
        }
        // GET: TrainingProgram/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TrainingProgram/Edit/5
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

        // GET: TrainingProgram/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrainingProgram/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}