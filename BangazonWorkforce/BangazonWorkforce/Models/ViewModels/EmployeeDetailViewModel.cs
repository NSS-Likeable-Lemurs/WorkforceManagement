using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeDetailViewModel
    {
        private readonly IConfiguration _config;

        public List<SelectListItem> TrainingProgram { get; set; }
        public Employee employee { get; set; }

        public EmployeeDetailViewModel() { }

        public EmployeeDetailViewModel(IConfiguration config)
        {

            using (IDbConnection conn = new SqlConnection(config.GetConnectionString("DefaultConnection")))
            {
                TrainingProgram = conn.Query<TrainingProgram>(@"
                    SELECT Id, Name FROM TrainingProgram;
                ")
                .AsEnumerable()
                .Select(li => new SelectListItem
                {
                    Text = li.Name,
                    Value = li.Id.ToString()
                }).ToList();
                ;
            }

            TrainingProgram.Insert(0, new SelectListItem
            {
                Text = "Choose training program...",
                Value = "0"
            });
        }
    }
}
