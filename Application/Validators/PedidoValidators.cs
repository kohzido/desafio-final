using Application.DTOs;
using FluentValidation;

namespace Application.Validators;

public class CreatePedidoRequestValidator : AbstractValidator<CreatePedidoRequest>
{
    public CreatePedidoRequestValidator()
    {
        RuleFor(x => x.ProdutoIds).NotNull().Must(ids => ids != null && ids.Count > 0)
            .WithMessage("ProdutoIds deve conter pelo menos 1 item.");
    }
}

public class UpdatePedidoRequestValidator : AbstractValidator<UpdatePedidoRequest>
{
    public UpdatePedidoRequestValidator()
    {
        RuleFor(x => x.ProdutoIds).NotNull().Must(ids => ids != null && ids.Count > 0)
            .WithMessage("ProdutoIds deve conter pelo menos 1 item.");
    }
}
