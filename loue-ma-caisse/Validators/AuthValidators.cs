using FluentValidation;
using loue_ma_caisse.DTOs;

namespace loue_ma_caisse.Validators;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est requis")
            .EmailAddress().WithMessage("Format d'email invalide");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Le mot de passe est requis")
            .MinimumLength(6).WithMessage("Le mot de passe doit contenir au moins 6 caractères");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Le prénom est requis");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Le nom est requis");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Le numéro de téléphone est requis")
            .Matches(@"^(?:(?:\+|00)33|0)\s*[1-9](?:[\s.-]*\d{2}){4}$")
            .WithMessage("Format de numéro de téléphone invalide");
    }
}

public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est requis")
            .EmailAddress().WithMessage("Format d'email invalide");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Le mot de passe est requis");
    }
}