using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

//Author: Helen Chalmers
//Purpose: ViewModel for the Department View

namespace BangazonWorkforce.Models.ViewModels
{
    public class DepartmentIndexViewModel
    {
        public Department department { get; set; }
    }
}
