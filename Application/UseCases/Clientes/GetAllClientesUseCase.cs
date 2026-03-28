using Application.DTOs;
using Domain.Interfaces;

namespace Application.UseCases.Clientes;

public class GetAllClientesUseCase
{
    private readonly IClienteRepository _clienteRepo;

    public GetAllClientesUseCase(IClienteRepository clienteRepo)
    {
        _clienteRepo = clienteRepo;
    }

    public async Task<IEnumerable<ClienteResponse>> ExecuteAsync()
    {
        var clientes = await _clienteRepo.GetAllAsync();
        return clientes.Select(c => new ClienteResponse(c.Id, c.Nome, c.Email));
    }
}
