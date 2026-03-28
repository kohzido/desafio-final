using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Produtos;

public class CreateProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepo;
    private readonly DbContext _dbContext;

    public CreateProdutoUseCase(IProdutoRepository produtoRepo, DbContext dbContext)
    {
        _produtoRepo = produtoRepo;
        _dbContext = dbContext;
    }

    public async Task<ProdutoResponse> ExecuteAsync(CreateProdutoRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var produto = new Produto(Guid.NewGuid(), request.Nome, request.Preco);
            await _produtoRepo.AddAsync(produto);
            await transaction.CommitAsync();

            return new ProdutoResponse(produto.Id, produto.Nome, produto.Preco);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
