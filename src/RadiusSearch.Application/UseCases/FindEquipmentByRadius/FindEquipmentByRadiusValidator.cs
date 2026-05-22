using FluentValidation;

namespace RadiusSearch.Application.UseCases.FindEquipmentByRadius;

public sealed class FindEquipmentByRadiusValidator
    : AbstractValidator<FindEquipmentByRadiusQuery>
{
    public FindEquipmentByRadiusValidator()
    {
        RuleFor(query => query.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("latitude must be between -90 and 90");

        RuleFor(query => query.RawLatitude)
            .Must(HaveAtLeastFiveDecimalPlaces)
            .WithMessage("latitude must have at least 5 decimal places");

        RuleFor(query => query.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("longitude must be between -180 and 180");

        RuleFor(query => query.RawLongitude)
            .Must(HaveAtLeastFiveDecimalPlaces)
            .WithMessage("longitude must have at least 5 decimal places");

        RuleFor(query => query.Radius)
            .InclusiveBetween(10, 1000)
            .WithMessage("radius must be between 10 and 1000 meters");

        RuleFor(query => query.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("page must be greater than or equal to 1");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 20)
            .WithMessage("pageSize must be between 1 and 20");
    }

    private static bool HaveAtLeastFiveDecimalPlaces(string raw)
    {
        var dotIndex = raw.IndexOf('.');

        return dotIndex >= 0 && raw.Length - dotIndex - 1 >= 5;
    }
}
