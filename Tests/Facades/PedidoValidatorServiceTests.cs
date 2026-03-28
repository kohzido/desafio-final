using Application.Exceptions;
using Application.Facades;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tests.Facades;

public class PedidoValidatorServiceTests
{
    private readonly Mock<IClienteRepository> _clienteRepoMock = new();
    private readonly Mock<IProdutoRepository> _produtoRepoMock = new();
    private readonly Mock<IPedidoRepository> _pedidoRepoMock = new();

    private PedidoValidatorService CreateService() =>
        new(_clienteRepoMock.Object, _produtoRepoMock.Object, _pedidoRepoMock.Object);

    [Fact]
    public async Task ValidarClienteAsync_DeveRetornarCliente_QuandoEncontrado()
    {
        var id = Guid.NewGuid();
        _clienteRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Cliente(id, "João", "joao@test.com"));

        var result = await CreateService().ValidarClienteAsync(id);

        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task ValidarClienteAsync_DeveLancarNotFoundException_QuandoClienteNaoEncontrado()
    {
        var id = Guid.NewGuid();
        _clienteRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Cliente?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => CreateService().ValidarClienteAsync(id));
    }

    [Fact]
    public async Task ValidarProdutosAsync_DeveRetornarProdutos_QuandoTodosEncontrados()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        _produtoRepoMock.Setup(r => r.GetByIdAsync(id1)).ReturnsAsync(new Produto(id1, "A", 10m));
        _produtoRepoMock.Setup(r => r.GetByIdAsync(id2)).ReturnsAsync(new Produto(id2, "B", 20m));

        var result = await CreateService().ValidarProdutosAsync(new List<Guid> { id1, id2 });

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task ValidarProdutosAsync_DeveLancarNotFoundException_QuandoProdutoNaoEncontrado()
    {
        var id = Guid.NewGuid();
        _produtoRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Produto?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateService().ValidarProdutosAsync(new List<Guid> { id }));
    }

    [Fact]
    public async Task ValidarPedidoAsync_DeveLancarNotFoundException_QuandoPedidoNaoEncontrado()
    {
        var id = Guid.NewGuid();
        _pedidoRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Pedido?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => CreateService().ValidarPedidoAsync(id));
    }
}
