using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeManagement.Models
{
    public class Country
    {
        public int Id { get; set; }

        [Required]
        public string CountryName { get; set; }
    }
}