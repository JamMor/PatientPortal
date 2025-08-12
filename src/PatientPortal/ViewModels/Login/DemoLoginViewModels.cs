using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class DemoLoginViewModel
    {
        public int DemoStaffId { get; set; }
        public string DemoStaffName { get; set; }
        public string DemoStaffRole { get; set; }
        public bool IsDemoStaffAdmin { get; set; }
    }

}