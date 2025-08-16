#nullable enable
using PatientPortal.Models;
using PatientPortal.Shared.Guard;

namespace PatientPortal.DTOs;

public record AccountDTO(
    string Username,
    string Password
);

public static class StaffFormViewAccountExtensions
{
    public static AccountDTO ToAccountDTO(this StaffFormView staffFormView)
    {
        return new AccountDTO(
            Guard.NotNull(staffFormView.StaffUsername, nameof(staffFormView.StaffUsername)),
            Guard.NotNull(staffFormView.Password, nameof(staffFormView.Password))
        );
    }
}