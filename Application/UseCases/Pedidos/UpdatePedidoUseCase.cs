using Application.DTOs;
using Application.Facades;
using Domain.Events;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Pedidos;

public class UpdatePedidoUseCase
{
    private readonly PedidoFacade _facade;
    private readonly IDomainEventPublisher _eventPublisher;
    private readonly IValidator<UpdatePedidoRequest> _validator;
    private readonly DbContext _dbContext;

    public UpdatePedidoUseCase(
        PedidoFacade facade,
        IDomainEventPublisher eventPublisher,
        IValidator<UpdatePedidoRequest> validator,
        DbContext dbContext)
    {
        _facade = facade;
        _eventPublisher = eventPublisher;
        _validator = validator;
        _dbContext = dbContext;
    }

    public async Task<PedidoResponse> ExecuteAsync(Guid id, UpdatePedidoRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var pedido = await _facade.AtualizarPedidoAsync(id, request.ProdutoIds);
            await _eventPublisher.PublishAsync(new PedidoAtualizado(pedido.Id, pedido.ValorTotal));
            await transaction.CommitAsync();

            return new PedidoResponse(
                pedido.Id,
                pedido.ClienteId,
                pedido.Produtos.Select(p => new ProdutoResponse(p.Id, p.Nome, p.Preco)).ToList(),
                pedido.ValorTotal);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
