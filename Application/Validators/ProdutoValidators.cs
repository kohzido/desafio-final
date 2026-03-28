using Application.DTOs;
using FluentValidation;

namespace Application.Validators;

public class CreateProdutoRequestValidator : AbstractValidator<CreateProdutoRequest>
{
    public CreateProdutoRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Preco).GreaterThan(0);
    }
}

public class UpdateProdutoRequestValidator : AbstractValidator<UpdateProdutoRequest>
{
    public UpdateProdutoRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Preco).GreaterThan(0);
    }
}
