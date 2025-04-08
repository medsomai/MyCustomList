using FluentValidation;

namespace MyCustomList.Validators;

public class ProductDtoValidator : AbstractValidator<ProductDto>
{
    public ProductDtoValidator()
    {
        RuleFor(product => product.Name)
            .NotEmpty()
            .WithMessage("Le nom du produit est requis.")
            .Length(1, 100)
            .WithMessage("Le nom du produit doit contenir entre 1 et 100 caractères.");

        // RuleFor(product => product.Description)
        //     .NotEmpty()
        //     .WithMessage("La description du produit est requise.")
        //     .Length(1, 500)
        //     .WithMessage("La description du produit doit contenir entre 1 et 500 caractères.");

        // RuleFor(product => product.Price)
        //     .GreaterThan(0)
        //     .WithMessage("Le prix du produit doit être supérieur à 0.");

        // RuleFor(product => product.ImageUrl)
        //     .NotEmpty()
        //     .WithMessage("L'URL de l'image du produit est requise.")
        //     .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
        //     .WithMessage("L'URL de l'image du produit doit être une URL valide.");
    }
}