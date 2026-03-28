using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Produtos;

public class CreateProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepo;
    private readonly DbContext _dbContext;
    private readonly IValidator<CreateProdutoRequest> _validator;

    public CreateProdutoUseCase(IProdutoRepository produtoRepo, DbContext dbContext, IValidator<CreateProdutoRequest> validator)
    {
        _produtoRepo = produtoRepo;
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<ProdutoResponse> ExecuteAsync(CreateProdutoRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

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
