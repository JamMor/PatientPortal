#nullable enable
using PatientPortal.Models;
using PatientPortal.Shared.Guard;

namespace PatientPortal.DTOs;

public record StaffDTO(
    string FirstName,
    string LastName,
    string Role
);

public static class StaffFormViewStaffExtensions
{
    public static StaffDTO ToStaffDTO(this StaffFormView staffFormView)
    {
        return new StaffDTO(
            Guard.NotNull(staffFormView.FirstName, nameof(staffFormView.FirstName)),
            Guard.NotNull(staffFormView.LastName, nameof(staffFormView.LastName)),
            Guard.NotNull(staffFormView.Role, nameof(staffFormView.Role))
        );
    }
}