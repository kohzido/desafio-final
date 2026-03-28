using Application.DTOs;
using Application.Exceptions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Clientes;

public class CreateClienteUseCase
{
    private readonly IClienteRepository _clienteRepo;
    private readonly DbContext _dbContext;

    public CreateClienteUseCase(IClienteRepository clienteRepo, DbContext dbContext)
    {
        _clienteRepo = clienteRepo;
        _dbContext = dbContext;
    }

    public async Task<ClienteResponse> ExecuteAsync(CreateClienteRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            if (await _clienteRepo.ExisteEmailAsync(request.Email))
                throw new BusinessRuleException("E-mail já está em uso por outro cliente.");

            var cliente = new Cliente(Guid.NewGuid(), request.Nome, request.Email);
            await _clienteRepo.AddAsync(cliente);
            await transaction.CommitAsync();

            return new ClienteResponse(cliente.Id, cliente.Nome, cliente.Email);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
