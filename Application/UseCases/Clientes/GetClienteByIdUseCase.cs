using Application.DTOs;
using Application.Exceptions;
using Domain.Interfaces;

namespace Application.UseCases.Clientes;

public class GetClienteByIdUseCase
{
    private readonly IClienteRepository _clienteRepo;

    public GetClienteByIdUseCase(IClienteRepository clienteRepo)
    {
        _clienteRepo = clienteRepo;
    }

    public async Task<ClienteResponse> ExecuteAsync(Guid id)
    {
        var cliente = await _clienteRepo.GetByIdAsync(id)
            ?? throw new NotFoundException("Cliente não encontrado.");

        return new ClienteResponse(cliente.Id, cliente.Nome, cliente.Email);
    }
}
