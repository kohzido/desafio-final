using Application.Exceptions;
using Application.UseCases.Clientes;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Tests.Helpers;
using Xunit;

namespace Tests.UseCases;

public class DeleteClienteUseCaseTests
{
    private readonly Mock<IClienteRepository> _clienteRepoMock = new();
    private readonly Mock<IPedidoRepository> _pedidoRepoMock = new();

    [Fact]
    public async Task ExecuteAsync_DeveDeletarCliente_QuandoNaoTiverPedidos()
    {
        var id = Guid.NewGuid();
        _clienteRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Cliente(id, "João", "joao@test.com"));
        _pedidoRepoMock.Setup(r => r.ExisteParaClienteAsync(id)).ReturnsAsync(false);
        _clienteRepoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

        using var dbContext = MockDbContextHelper.CreateInMemoryDbContext();
        var useCase = new DeleteClienteUseCase(_clienteRepoMock.Object, _pedidoRepoMock.Object, dbContext);

        await useCase.ExecuteAsync(id);

        _clienteRepoMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarNotFoundException_QuandoClienteNaoEncontrado()
    {
        var id = Guid.NewGuid();
        _clienteRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Cliente?)null);

        using var dbContext = MockDbContextHelper.CreateInMemoryDbContext();
        var useCase = new DeleteClienteUseCase(_clienteRepoMock.Object, _pedidoRepoMock.Object, dbContext);

        await Assert.ThrowsAsync<NotFoundException>(() => useCase.ExecuteAsync(id));
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarBusinessRuleException_QuandoClienteTemPedidos()
    {
        var id = Guid.NewGuid();
        _clienteRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Cliente(id, "João", "joao@test.com"));
        _pedidoRepoMock.Setup(r => r.ExisteParaClienteAsync(id)).ReturnsAsync(true);

        using var dbContext = MockDbContextHelper.CreateInMemoryDbContext();
        var useCase = new DeleteClienteUseCase(_clienteRepoMock.Object, _pedidoRepoMock.Object, dbContext);

        await Assert.ThrowsAsync<BusinessRuleException>(() => useCase.ExecuteAsync(id));
    }
}
