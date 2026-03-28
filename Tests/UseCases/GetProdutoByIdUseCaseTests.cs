using Application.Exceptions;
using Application.UseCases.Produtos;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tests.UseCases;

public class GetProdutoByIdUseCaseTests
{
    private readonly Mock<IProdutoRepository> _produtoRepoMock = new();

    [Fact]
    public async Task ExecuteAsync_DeveRetornarProduto_QuandoEncontrado()
    {
        var id = Guid.NewGuid();
        _produtoRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Produto(id, "Notebook", 3500m));

        var useCase = new GetProdutoByIdUseCase(_produtoRepoMock.Object);
        var result = await useCase.ExecuteAsync(id);

        Assert.Equal(id, result.Id);
        Assert.Equal("Notebook", result.Nome);
        Assert.Equal(3500m, result.Preco);
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarNotFoundException_QuandoNaoEncontrado()
    {
        var id = Guid.NewGuid();
        _produtoRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Produto?)null);

        var useCase = new GetProdutoByIdUseCase(_produtoRepoMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => useCase.ExecuteAsync(id));
    }
}
