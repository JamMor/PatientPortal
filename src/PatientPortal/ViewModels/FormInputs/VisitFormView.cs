// Form Input Model
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PatientPortal.Models
{
    public class VisitFormView
    {
        public string? Comment { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Visit")]
        public DateTime DateOfVisit { get; set; } = DateTime.Today;

        public List<HealthIssueCheckbox> HealthIssues { get; set; } = [];
    }

    public static class VisitFormViewExtensions
    {
        public static VisitFormView ApplyInput(this VisitFormView form, VisitFormInput input)
        {
            form.Comment = input.Comment;
            form.DateOfVisit = input.DateOfVisit;
            foreach (var checkbox in form.HealthIssues)
            {
                var selection = input.HealthIssues
                    .FirstOrDefault(s => s.HealthIssueId == checkbox.HealthIssueId);
                if (selection != null)
                {
                    checkbox.Selected = selection.Selected;
                }
            }
            return form;
        }
    }
}
