// Form Input Model
using System.Collections.Generic;
using System.Linq;

namespace PatientPortal.Models
{
    public class TestResultFormView
    {
        public string? Type { get; set; }

        public string? Comment { get; set; }

        public List<HealthIssueCheckbox> HealthIssues { get; set; } = [];
    }

    public static class TestResultFormViewExtensions
    {
        public static TestResultFormView ApplyInput(this TestResultFormView form, TestResultFormInput input)
        {
            form.Type = input.Type;
            form.Comment = input.Comment;
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
