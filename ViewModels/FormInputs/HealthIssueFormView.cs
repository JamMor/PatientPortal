using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    public class HealthIssueFormView
    {
        public PatientHeaderInfoView Patient{ get; set; }
        public HealthIssue HealthIssue { get; set; }
    }
}