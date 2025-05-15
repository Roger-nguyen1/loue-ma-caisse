using FluentValidation;
using loue_ma_caisse.DTOs;

namespace loue_ma_caisse.Validators;

public class CreateVehicleDtoValidator : AbstractValidator<CreateVehicleDto>
{
    public CreateVehicleDtoValidator()
    {
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("La marque est requise");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Le modèle est requis");

        RuleFor(x => x.Year)
            .NotEmpty().WithMessage("L'année est requise")
            .InclusiveBetween(1900, DateTime.Now.Year + 1)
            .WithMessage($"L'année doit être comprise entre 1900 et {DateTime.Now.Year + 1}");

        RuleFor(x => x.Transmission)
            .NotEmpty().WithMessage("Le type de transmission est requis")
            .Must(x => x == "Manual" || x == "Automatic")
            .WithMessage("La transmission doit être 'Manual' ou 'Automatic'");

        RuleFor(x => x.FuelType)
            .NotEmpty().WithMessage("Le type de carburant est requis")
            .Must(x => new[] { "Essence", "Diesel", "Électrique", "Hybride" }.Contains(x))
            .WithMessage("Le type de carburant doit être 'Essence', 'Diesel', 'Électrique' ou 'Hybride'");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("La ville est requise");

        RuleFor(x => x.PricePerDay)
            .NotEmpty().WithMessage("Le prix par jour est requis")
            .GreaterThan(0).WithMessage("Le prix par jour doit être supérieur à 0");
    }
}

public class UpdateVehicleDtoValidator : AbstractValidator<UpdateVehicleDto>
{
    public UpdateVehicleDtoValidator()
    {
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("La marque est requise");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Le modèle est requis");

        RuleFor(x => x.Year)
            .NotEmpty().WithMessage("L'année est requise")
            .InclusiveBetween(1900, DateTime.Now.Year + 1)
            .WithMessage($"L'année doit être comprise entre 1900 et {DateTime.Now.Year + 1}");

        RuleFor(x => x.Transmission)
            .NotEmpty().WithMessage("Le type de transmission est requis")
            .Must(x => x == "Manual" || x == "Automatic")
            .WithMessage("La transmission doit être 'Manual' ou 'Automatic'");

        RuleFor(x => x.FuelType)
            .NotEmpty().WithMessage("Le type de carburant est requis")
            .Must(x => new[] { "Essence", "Diesel", "Électrique", "Hybride" }.Contains(x))
            .WithMessage("Le type de carburant doit être 'Essence', 'Diesel', 'Électrique' ou 'Hybride'");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("La ville est requise");

        RuleFor(x => x.PricePerDay)
            .NotEmpty().WithMessage("Le prix par jour est requis")
            .GreaterThan(0).WithMessage("Le prix par jour doit être supérieur à 0");
    }
}