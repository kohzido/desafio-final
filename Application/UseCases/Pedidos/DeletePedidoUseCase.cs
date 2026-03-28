using Application.Facades;
using Domain.Events;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Pedidos;

public class DeletePedidoUseCase
{
    private readonly PedidoFacade _facade;
    private readonly IDomainEventPublisher _eventPublisher;
    private readonly DbContext _dbContext;

    public DeletePedidoUseCase(
        PedidoFacade facade,
        IDomainEventPublisher eventPublisher,
        DbContext dbContext)
    {
        _facade = facade;
        _eventPublisher = eventPublisher;
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(Guid id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await _facade.RemoverPedidoAsync(id);
            await _eventPublisher.PublishAsync(new PedidoRemovido(id));
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
