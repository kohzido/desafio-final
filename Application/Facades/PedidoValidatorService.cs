using Application.Exceptions;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Facades;

public class PedidoValidatorService
{
    private readonly IClienteRepository _clienteRepo;
    private readonly IProdutoRepository _produtoRepo;
    private readonly IPedidoRepository _pedidoRepo;

    public PedidoValidatorService(
        IClienteRepository clienteRepo,
        IProdutoRepository produtoRepo,
        IPedidoRepository pedidoRepo)
    {
        _clienteRepo = clienteRepo;
        _produtoRepo = produtoRepo;
        _pedidoRepo = pedidoRepo;
    }

    public virtual async Task<Cliente> ValidarClienteAsync(Guid clienteId)
    {
        return await _clienteRepo.GetByIdAsync(clienteId)
            ?? throw new NotFoundException("Cliente não encontrado.");
    }

    public virtual async Task<List<Produto>> ValidarProdutosAsync(List<Guid> produtoIds)
    {
        var produtos = new List<Produto>();
        foreach (var id in produtoIds)
        {
            var produto = await _produtoRepo.GetByIdAsync(id)
                ?? throw new NotFoundException($"Produto {id} não encontrado.");
            produtos.Add(produto);
        }
        return produtos;
    }

    public virtual async Task<Pedido> ValidarPedidoAsync(Guid pedidoId)
    {
        return await _pedidoRepo.GetByIdAsync(pedidoId)
            ?? throw new NotFoundException("Pedido não encontrado.");
    }
}
