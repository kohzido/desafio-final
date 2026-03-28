using Application.DTOs;
using Domain.Interfaces;

namespace Application.UseCases.Pedidos;

public class GetAllPedidosUseCase
{
    private readonly IPedidoRepository _pedidoRepo;

    public GetAllPedidosUseCase(IPedidoRepository pedidoRepo)
    {
        _pedidoRepo = pedidoRepo;
    }

    public async Task<IEnumerable<PedidoResponse>> ExecuteAsync()
    {
        var pedidos = await _pedidoRepo.GetAllAsync();
        return pedidos.Select(pedido => new PedidoResponse(
            pedido.Id,
            pedido.ClienteId,
            pedido.Produtos.Select(p => new ProdutoResponse(p.Id, p.Nome, p.Preco)).ToList(),
            pedido.ValorTotal));
    }
}
