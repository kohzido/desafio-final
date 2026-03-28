using Application.Exceptions;
using Application.UseCases.Clientes;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tests.UseCases;

public class GetClienteByIdUseCaseTests
{
    private readonly Mock<IClienteRepository> _clienteRepoMock = new();

    [Fact]
    public async Task ExecuteAsync_DeveRetornarCliente_QuandoEncontrado()
    {
        var id = Guid.NewGuid();
        _clienteRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Cliente(id, "João", "joao@test.com"));

        var useCase = new GetClienteByIdUseCase(_clienteRepoMock.Object);
        var result = await useCase.ExecuteAsync(id);

        Assert.Equal(id, result.Id);
        Assert.Equal("João", result.Nome);
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarNotFoundException_QuandoNaoEncontrado()
    {
        var id = Guid.NewGuid();
        _clienteRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Cliente?)null);

        var useCase = new GetClienteByIdUseCase(_clienteRepoMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => useCase.ExecuteAsync(id));
    }
}
