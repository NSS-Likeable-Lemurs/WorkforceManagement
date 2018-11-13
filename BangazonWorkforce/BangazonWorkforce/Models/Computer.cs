using System;
using System.ComponentModel.DataAnnotations;

namespace BangazonWorkforce.Models
{
    public class Computer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must provide a purchase date for this computer.")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "You must provide a decomission date for this computer.")]
        public DateTime DecomissionDate { get; set; }

        [Required(ErrorMessage = "You must provide a make for this computer.")]
        public string Make { get; set; }

        [Required(ErrorMessage = "You must provide a manufacturer for this computer.")]
        public string Manufacturer { get; set; }
    }
}
