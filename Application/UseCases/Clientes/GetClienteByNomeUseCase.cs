using Application.DTOs;
using Domain.Interfaces;

namespace Application.UseCases.Clientes;

public class GetClienteByNomeUseCase
{
    private readonly IClienteRepository _clienteRepo;

    public GetClienteByNomeUseCase(IClienteRepository clienteRepo)
    {
        _clienteRepo = clienteRepo;
    }

    public async Task<IEnumerable<ClienteResponse>> ExecuteAsync(string nome)
    {
        var clientes = await _clienteRepo.GetByNomeAsync(nome);
        return clientes.Select(c => new ClienteResponse(c.Id, c.Nome, c.Email));
    }
}
