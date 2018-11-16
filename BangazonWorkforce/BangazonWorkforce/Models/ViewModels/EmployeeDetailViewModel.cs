using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class EmployeeDetailViewModel
    {
        public Employee Employee { get; set; }
        public List<Computer> AllComputers { get; set; }
        public List<TrainingProgram> TrainingPrograms { get; set; }
    }
}
