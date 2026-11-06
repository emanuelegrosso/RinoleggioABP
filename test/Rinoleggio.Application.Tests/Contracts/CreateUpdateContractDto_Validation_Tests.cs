using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Rinoleggio.Contracts;
using Rinoleggio.Listings;
using Shouldly;
using Xunit;

namespace Rinoleggio.Contracts;

public class CreateUpdateContractDto_Validation_Tests
{
    [Fact]
    public void Should_Be_Valid_With_FuelType_And_Transmission_Zero()
    {
        var dto = ValidDto();
        dto.FuelType = FuelType.Petrol;
        dto.Transmission = Transmission.Manual;

        var results = Validate(dto);

        results.ShouldBeEmpty();
    }

    private static CreateUpdateContractDto ValidDto()
    {
        return new CreateUpdateContractDto
        {
            Title = "Test annuncio",
            ProvinceId = Guid.NewGuid(),
            RentalCompanyId = Guid.NewGuid(),
            BrandId = Guid.NewGuid(),
            Model = "Modello",
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)),
            MonthlyFee = 300m,
            TotalKm = 60000,
            CurrentKm = 10000,
            RegistrationYear = 2023,
            FuelType = FuelType.Diesel,
            Transmission = Transmission.Automatic
        };
    }

    private static List<ValidationResult> Validate(CreateUpdateContractDto dto)
    {
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
        return results;
    }
}
