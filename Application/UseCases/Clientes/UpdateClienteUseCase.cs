using Application.DTOs;
using Application.Exceptions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Clientes;

public class UpdateClienteUseCase
{
    private readonly IClienteRepository _clienteRepo;
    private readonly DbContext _dbContext;

    public UpdateClienteUseCase(IClienteRepository clienteRepo, DbContext dbContext)
    {
        _clienteRepo = clienteRepo;
        _dbContext = dbContext;
    }

    public async Task<ClienteResponse> ExecuteAsync(Guid id, UpdateClienteRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var existente = await _clienteRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Cliente não encontrado.");

            if (await _clienteRepo.ExisteEmailAsync(request.Email, excludeId: id))
                throw new BusinessRuleException("E-mail já está em uso por outro cliente.");

            var atualizado = existente with { Nome = request.Nome, Email = request.Email };
            await _clienteRepo.UpdateAsync(atualizado);
            await transaction.CommitAsync();

            return new ClienteResponse(atualizado.Id, atualizado.Nome, atualizado.Email);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
