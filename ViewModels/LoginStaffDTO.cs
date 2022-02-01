namespace PatientPortal.Models
{
public class LoginStaffDTO
    {
        public int StaffId { get; set; }
        public int MessagingLinkId { get; set; }
        public bool IsAdmin { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}