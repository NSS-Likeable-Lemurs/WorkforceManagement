using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BangazonWorkforce.Models
{
    public class EmployeeAddEditViewModel
    {
        public Employee Employee { get; set; }
        public List<Department> AllDepartments { get; set; }
        public List<SelectListItem> AllDepartmentOptions
        {
            get
            {
                if (AllDepartments == null)
                {
                    return null;
                }

                return AllDepartments
                        .Select((d) => new SelectListItem(d.Name, d.Id.ToString()))
                        .ToList();
            }
        }
        public List<Computer> AllComputers { get; set; }
        public List<SelectListItem> AllComputerOptions
        {
            get
            {
                if (AllComputers == null)
                {
                    return null;
                }

                return AllComputers
                        .Select((c) => new SelectListItem(c.Make, c.Id.ToString()))
                        .ToList();
            }
        }
        //public List<TrainingProgram> AllTrainingPrograms { get; set; }
        //public List<SelectListItem> AllTrainingProgramOptions
        //{
        //    get
        //    {
        //        if (AllTrainingPrograms == null)
        //        {
        //            return null;
        //        }

        //        return AllTrainingPrograms
        //                .Select((tp) => new SelectListItem(tp.Name, tp.Id.ToString()))
        //                .ToList();
        //    }
        //}
    }
}
