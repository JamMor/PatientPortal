#nullable enable
using System;
using PatientPortal.Models;
using PatientPortal.Shared.Guard;

namespace PatientPortal.DTOs;

public record PatientDTO(
    string FirstName,
    string LastName,
    DateTime DOB,
    string Last4SSN,
    string? PhoneNumber,
    string? Email,
    AddressDTO? Address
);

public static class PatientFormViewPatientExtensions
{
    public static PatientDTO ToPatientDTO(this PatientFormView patientFormView)
    {
        return new PatientDTO(
            Guard.NotNull(patientFormView.FirstName, nameof(patientFormView.FirstName)),
            Guard.NotNull(patientFormView.LastName, nameof(patientFormView.LastName)),
            patientFormView.DOB,
            Guard.NotNull(patientFormView.Last4SSN, nameof(patientFormView.Last4SSN)),
            patientFormView.PhoneNumber,
            patientFormView.Email,
            patientFormView.Address?.ToAddressDTO()
        );
    }
}