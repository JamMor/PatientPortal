namespace PatientPortal.Models
{
    public class InboxType
    {
        public static readonly InboxType Patient = new("patient", "Patient", withPatient: true);
        public static readonly InboxType Staff = new("staff", "Staff", withPatient: false);

        private InboxType(string route, string label, bool withPatient)
        {
            Route = route;
            Label = label;
            WithPatient = withPatient;
        }

        public string Route { get; }
        public string Label { get; }
        public bool WithPatient { get; }

        public static InboxType? FromRoute(string? route) =>
            route == Patient.Route ? Patient
            : route == Staff.Route ? Staff
            : null;
    }
}
