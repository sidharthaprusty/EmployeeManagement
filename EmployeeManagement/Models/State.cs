using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeManagement.Models
{
    public class State
    {
        public int Id { get; set; }

        [Required]
        public string StateName { get; set; }

        public virtual Country Country { get; set; }
    }
}