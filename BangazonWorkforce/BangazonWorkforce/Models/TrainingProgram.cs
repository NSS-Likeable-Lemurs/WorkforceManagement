﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BangazonWorkforce.Models
{
    public class TrainingProgram
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must provide a name for this training program.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must provide a description for this training program.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "You must provide a start date for this training program.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "You must provide a end date for this training program.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "You must provide an attendance number for this training program.")]
        public int MaxAttendees { get; set; }
    }
}
