using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class TestLoginViewModel
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string TestRole { get; set; }
    }

}