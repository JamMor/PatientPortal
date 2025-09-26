using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class DemoLoginViewModel
    {
        public int DemoStaffId { get; set; }
        public required string DemoStaffName { get; set; }
        public required string DemoStaffRole { get; set; }
        public bool IsDemoStaffAdmin { get; set; }
    }
}
