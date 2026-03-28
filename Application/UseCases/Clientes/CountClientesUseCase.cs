using Domain.Interfaces;

namespace Application.UseCases.Clientes;

public class CountClientesUseCase
{
    private readonly IClienteRepository _clienteRepo;

    public CountClientesUseCase(IClienteRepository clienteRepo)
    {
        _clienteRepo = clienteRepo;
    }

    public async Task<int> ExecuteAsync()
    {
        return await _clienteRepo.CountAsync();
    }
}
