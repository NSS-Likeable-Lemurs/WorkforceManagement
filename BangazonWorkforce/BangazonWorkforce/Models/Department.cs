using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//Author: Helen Chalmers
// Purpose: A Model for the Department Database
//
//
namespace BangazonWorkforce.Models
{

    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must provide a name for this department.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must provide a budget for this department.")]
        [Range(0, int.MaxValue, ErrorMessage = "A budget cannot be less than zero.")]
        public int Budget { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();


        public int TotalEmployees { get; set; }
    }
}
