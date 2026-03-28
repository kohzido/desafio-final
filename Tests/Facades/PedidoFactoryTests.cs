using Application.Facades;
using Domain.Entities;
using Xunit;

namespace Tests.Facades;

public class PedidoFactoryTests
{
    private readonly PedidoFactory _factory = new();

    [Fact]
    public void Criar_DeveCalcularValorTotalCorreto()
    {
        var clienteId = Guid.NewGuid();
        var produtos = new List<Produto>
        {
            new Produto(Guid.NewGuid(), "A", 10m),
            new Produto(Guid.NewGuid(), "B", 20m),
            new Produto(Guid.NewGuid(), "C", 30m)
        };

        var pedido = _factory.Criar(clienteId, produtos);

        Assert.Equal(60m, pedido.ValorTotal);
        Assert.Equal(clienteId, pedido.ClienteId);
        Assert.Equal(3, pedido.Produtos.Count);
        Assert.NotEqual(Guid.Empty, pedido.Id);
    }

    [Fact]
    public void Criar_DeveConterSnapshotDeProdutos()
    {
        var clienteId = Guid.NewGuid();
        var prod1 = new Produto(Guid.NewGuid(), "Produto X", 99m);
        var produtos = new List<Produto> { prod1 };

        var pedido = _factory.Criar(clienteId, produtos);

        Assert.Single(pedido.Produtos);
        Assert.Equal("Produto X", pedido.Produtos[0].Nome);
        Assert.Equal(99m, pedido.Produtos[0].Preco);
    }

    [Fact]
    public void Atualizar_DeveRecalcularValorTotal()
    {
        var clienteId = Guid.NewGuid();
        var pedido = _factory.Criar(clienteId, new List<Produto>
        {
            new Produto(Guid.NewGuid(), "A", 10m)
        });

        var novosProdutos = new List<Produto>
        {
            new Produto(Guid.NewGuid(), "B", 50m),
            new Produto(Guid.NewGuid(), "C", 75m)
        };

        _factory.Atualizar(pedido, novosProdutos);

        Assert.Equal(125m, pedido.ValorTotal);
        Assert.Equal(2, pedido.Produtos.Count);
    }

    [Fact]
    public void Atualizar_DeveModificarInPlace()
    {
        var clienteId = Guid.NewGuid();
        var pedido = _factory.Criar(clienteId, new List<Produto>
        {
            new Produto(Guid.NewGuid(), "Original", 10m)
        });
        var pedidoRef = pedido;

        _factory.Atualizar(pedido, new List<Produto> { new Produto(Guid.NewGuid(), "Novo", 20m) });

        Assert.Same(pedidoRef, pedido);
        Assert.Equal(20m, pedido.ValorTotal);
    }
}
