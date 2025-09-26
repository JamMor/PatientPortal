using PatientPortal.Models;
using PatientPortal.Shared.Guard;

namespace PatientPortal.DTOs;

public record LoginStaffDTO(
    string StaffUsername,
    string LoginPassword
);

public static class LoginStaffFormViewExtensions
{
    public static LoginStaffDTO ToLoginStaffDTO(this LoginStaff loginStaff)
    {
        return new LoginStaffDTO(
            Guard.NotNull(loginStaff.StaffUsername, nameof(loginStaff.StaffUsername)),
            Guard.NotNull(loginStaff.LoginPassword, nameof(loginStaff.LoginPassword))
        );
    }
}