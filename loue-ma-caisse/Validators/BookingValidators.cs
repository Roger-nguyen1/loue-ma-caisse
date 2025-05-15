using FluentValidation;
using loue_ma_caisse.DTOs;

namespace loue_ma_caisse.Validators;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage("L'identifiant du véhicule est requis");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("La date de début est requise")
            .Must(date => date >= DateTime.Today)
            .WithMessage("La date de début doit être aujourd'hui ou dans le futur");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("La date de fin est requise")
            .Must((booking, endDate) => endDate > booking.StartDate)
            .WithMessage("La date de fin doit être postérieure à la date de début");

        RuleFor(x => x)
            .Must(booking => (booking.EndDate - booking.StartDate).TotalDays <= 30)
            .WithMessage("La durée maximale de location est de 30 jours");
    }
}