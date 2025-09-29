using System;
using System.Linq;
using PatientPortal.Models;
using PatientPortal.Shared.Guard;

namespace PatientPortal.DTOs;

public record AddressDTO(
    string StreetAddress,
    string City,
    string State,
    string ZipCode
);

public static class AddressFormViewAddressExtensions
{
    public static AddressDTO? ToAddressDTOOrNull(this AddressFormView? addressFormView)
    {
        if (addressFormView is null) return null;

        string?[] fields = [addressFormView.StreetAddress, addressFormView.City, addressFormView.State, addressFormView.ZipCode];

        if (fields.All(f => f is null)) return null;
        if (fields.Any(f => f is null)) throw new ArgumentException("Address must be fully provided or omitted entirely.");

        return new AddressDTO(
            Guard.NotNull(addressFormView.StreetAddress, nameof(addressFormView.StreetAddress)),
            Guard.NotNull(addressFormView.City, nameof(addressFormView.City)),
            Guard.NotNull(addressFormView.State, nameof(addressFormView.State)),
            Guard.NotNull(addressFormView.ZipCode, nameof(addressFormView.ZipCode))
        );
    }
}