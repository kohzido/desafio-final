using Application.Exceptions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Clientes;

public class DeleteClienteUseCase
{
    private readonly IClienteRepository _clienteRepo;
    private readonly IPedidoRepository _pedidoRepo;
    private readonly DbContext _dbContext;

    public DeleteClienteUseCase(
        IClienteRepository clienteRepo,
        IPedidoRepository pedidoRepo,
        DbContext dbContext)
    {
        _clienteRepo = clienteRepo;
        _pedidoRepo = pedidoRepo;
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(Guid id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var existente = await _clienteRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Cliente não encontrado.");

            if (await _pedidoRepo.ExisteParaClienteAsync(id))
                throw new BusinessRuleException("Cliente não pode ser removido pois possui pedidos associados.");

            await _clienteRepo.DeleteAsync(id);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
