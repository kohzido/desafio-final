using Application.DTOs;
using Application.Exceptions;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Produtos;

public class UpdateProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepo;
    private readonly DbContext _dbContext;
    private readonly IValidator<UpdateProdutoRequest> _validator;

    public UpdateProdutoUseCase(IProdutoRepository produtoRepo, DbContext dbContext, IValidator<UpdateProdutoRequest> validator)
    {
        _produtoRepo = produtoRepo;
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<ProdutoResponse> ExecuteAsync(Guid id, UpdateProdutoRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

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
