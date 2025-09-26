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
    public static AddressDTO ToAddressDTO(this AddressFormView addressFormView)
    {
        return new AddressDTO(
            Guard.NotNull(addressFormView.StreetAddress, nameof(addressFormView.StreetAddress)),
            Guard.NotNull(addressFormView.City, nameof(addressFormView.City)),
            Guard.NotNull(addressFormView.State, nameof(addressFormView.State)),
            Guard.NotNull(addressFormView.ZipCode, nameof(addressFormView.ZipCode))
        );
    }
}