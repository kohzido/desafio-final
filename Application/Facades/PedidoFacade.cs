using Domain.Entities;
using Domain.Interfaces;

namespace Application.Facades;

public class PedidoFacade
{
    private readonly PedidoValidatorService _validator;
    private readonly PedidoFactory _factory;
    private readonly IPedidoRepository _pedidoRepo;

    public PedidoFacade(
        PedidoValidatorService validator,
        PedidoFactory factory,
        IPedidoRepository pedidoRepo)
    {
        _validator = validator;
        _factory = factory;
        _pedidoRepo = pedidoRepo;
    }

    public virtual async Task<Pedido> CriarPedidoAsync(Guid clienteId, List<Guid> produtoIds)
    {
        await _validator.ValidarClienteAsync(clienteId);
        var produtos = await _validator.ValidarProdutosAsync(produtoIds);
        var pedido = _factory.Criar(clienteId, produtos);
        await _pedidoRepo.AddAsync(pedido);
        return pedido;
    }

    public virtual async Task<Pedido> AtualizarPedidoAsync(Guid pedidoId, List<Guid> produtoIds)
    {
        var pedido = await _validator.ValidarPedidoAsync(pedidoId);
        var produtos = await _validator.ValidarProdutosAsync(produtoIds);
        _factory.Atualizar(pedido, produtos);
        await _pedidoRepo.UpdateAsync(pedido);
        return pedido;
    }

    public virtual async Task RemoverPedidoAsync(Guid pedidoId)
    {
        await _validator.ValidarPedidoAsync(pedidoId);
        await _pedidoRepo.DeleteAsync(pedidoId);
    }
}
