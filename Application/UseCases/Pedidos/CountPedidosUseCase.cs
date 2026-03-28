using Domain.Interfaces;

namespace Application.UseCases.Pedidos;

public class CountPedidosUseCase
{
    private readonly IPedidoRepository _pedidoRepo;

    public CountPedidosUseCase(IPedidoRepository pedidoRepo)
    {
        _pedidoRepo = pedidoRepo;
    }

    public async Task<int> ExecuteAsync()
    {
        return await _pedidoRepo.CountAsync();
    }
}
