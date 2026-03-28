using Application.Exceptions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Produtos;

public class DeleteProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepo;
    private readonly IPedidoRepository _pedidoRepo;
    private readonly DbContext _dbContext;

    public DeleteProdutoUseCase(
        IProdutoRepository produtoRepo,
        IPedidoRepository pedidoRepo,
        DbContext dbContext)
    {
        _produtoRepo = produtoRepo;
        _pedidoRepo = pedidoRepo;
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(Guid id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var existente = await _produtoRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Produto não encontrado.");

            if (await _pedidoRepo.ExisteParaProdutoAsync(id))
                throw new BusinessRuleException("Produto não pode ser removido pois está associado a pedidos existentes.");

            await _produtoRepo.DeleteAsync(id);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
