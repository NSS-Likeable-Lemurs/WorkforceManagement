using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



//Author: Priyanka Garg
//Purpose: ViewModel for the Department Detail
namespace BangazonWorkforce.Models.ViewModels
{
    public class DepartmentDetailViewModel
    {
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public int Id { get; set; }
        public string Name { get; set; }
        //public Department department { get; set; }

    }
}
