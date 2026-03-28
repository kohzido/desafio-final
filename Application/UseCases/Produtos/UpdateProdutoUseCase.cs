using Application.DTOs;
using Application.Exceptions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Produtos;

public class UpdateProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepo;
    private readonly DbContext _dbContext;

    public UpdateProdutoUseCase(IProdutoRepository produtoRepo, DbContext dbContext)
    {
        _produtoRepo = produtoRepo;
        _dbContext = dbContext;
    }

    public async Task<ProdutoResponse> ExecuteAsync(Guid id, UpdateProdutoRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var existente = await _produtoRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Produto não encontrado.");

            var atualizado = existente with { Nome = request.Nome, Preco = request.Preco };
            await _produtoRepo.UpdateAsync(atualizado);
            await transaction.CommitAsync();

            return new ProdutoResponse(atualizado.Id, atualizado.Nome, atualizado.Preco);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
