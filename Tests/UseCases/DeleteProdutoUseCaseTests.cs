using Application.Exceptions;
using Application.UseCases.Produtos;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Tests.Helpers;
using Xunit;

namespace Tests.UseCases;

public class DeleteProdutoUseCaseTests
{
    private readonly Mock<IProdutoRepository> _produtoRepoMock = new();
    private readonly Mock<IPedidoRepository> _pedidoRepoMock = new();

    [Fact]
    public async Task ExecuteAsync_DeveDeletarProduto_QuandoNaoAssociadoAPedidos()
    {
        var id = Guid.NewGuid();
        _produtoRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Produto(id, "Produto A", 10m));
        _pedidoRepoMock.Setup(r => r.ExisteParaProdutoAsync(id)).ReturnsAsync(false);
        _produtoRepoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

        using var dbContext = MockDbContextHelper.CreateInMemoryDbContext();
        var useCase = new DeleteProdutoUseCase(_produtoRepoMock.Object, _pedidoRepoMock.Object, dbContext);

        await useCase.ExecuteAsync(id);

        _produtoRepoMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarNotFoundException_QuandoProdutoNaoEncontrado()
    {
        var id = Guid.NewGuid();
        _produtoRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Produto?)null);

        using var dbContext = MockDbContextHelper.CreateInMemoryDbContext();
        var useCase = new DeleteProdutoUseCase(_produtoRepoMock.Object, _pedidoRepoMock.Object, dbContext);

        await Assert.ThrowsAsync<NotFoundException>(() => useCase.ExecuteAsync(id));
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarBusinessRuleException_QuandoProdutoTemPedidos()
    {
        var id = Guid.NewGuid();
        _produtoRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Produto(id, "Produto A", 10m));
        _pedidoRepoMock.Setup(r => r.ExisteParaProdutoAsync(id)).ReturnsAsync(true);

        using var dbContext = MockDbContextHelper.CreateInMemoryDbContext();
        var useCase = new DeleteProdutoUseCase(_produtoRepoMock.Object, _pedidoRepoMock.Object, dbContext);

        await Assert.ThrowsAsync<BusinessRuleException>(() => useCase.ExecuteAsync(id));
    }
}
