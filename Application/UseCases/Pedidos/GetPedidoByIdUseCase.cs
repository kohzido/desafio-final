using Application.DTOs;
using Application.Exceptions;
using Domain.Interfaces;

namespace Application.UseCases.Pedidos;

public class GetPedidoByIdUseCase
{
    private readonly IPedidoRepository _pedidoRepo;

    public GetPedidoByIdUseCase(IPedidoRepository pedidoRepo)
    {
        _pedidoRepo = pedidoRepo;
    }

    public async Task<PedidoResponse> ExecuteAsync(Guid id)
    {
        var pedido = await _pedidoRepo.GetByIdAsync(id)
            ?? throw new NotFoundException("Pedido não encontrado.");

        return new PedidoResponse(
            pedido.Id,
            pedido.ClienteId,
            pedido.Produtos.Select(p => new ProdutoResponse(p.Id, p.Nome, p.Preco)).ToList(),
            pedido.ValorTotal);
    }
}
