using Application.DTOs;
using Application.Exceptions;
using Application.UseCases.Clientes;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Tests.Helpers;
using Xunit;

namespace Tests.UseCases;

public class UpdateClienteUseCaseTests
{
    private readonly Mock<IClienteRepository> _clienteRepoMock = new();

    [Fact]
    public async Task ExecuteAsync_DeveRetornarClienteAtualizado_QuandoDadosValidos()
    {
        var id = Guid.NewGuid();
        _clienteRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Cliente(id, "Nome Antigo", "antigo@test.com"));
        _clienteRepoMock.Setup(r => r.ExisteEmailAsync("novo@test.com", id))
            .ReturnsAsync(false);
        _clienteRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Cliente>()))
            .Returns(Task.CompletedTask);

        using var dbContext = MockDbContextHelper.CreateInMemoryDbContext();
        var useCase = new UpdateClienteUseCase(_clienteRepoMock.Object, dbContext);

        var result = await useCase.ExecuteAsync(id, new UpdateClienteRequest("Nome Novo", "novo@test.com"));

        Assert.Equal(id, result.Id);
        Assert.Equal("Nome Novo", result.Nome);
        Assert.Equal("novo@test.com", result.Email);
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarNotFoundException_QuandoClienteNaoEncontrado()
    {
        var id = Guid.NewGuid();
        _clienteRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Cliente?)null);

        using var dbContext = MockDbContextHelper.CreateInMemoryDbContext();
        var useCase = new UpdateClienteUseCase(_clienteRepoMock.Object, dbContext);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            useCase.ExecuteAsync(id, new UpdateClienteRequest("Nome", "email@test.com")));
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarBusinessRuleException_QuandoEmailEmUso()
    {
        var id = Guid.NewGuid();
        _clienteRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Cliente(id, "João", "joao@test.com"));
        _clienteRepoMock.Setup(r => r.ExisteEmailAsync("outro@test.com", id))
            .ReturnsAsync(true);

        using var dbContext = MockDbContextHelper.CreateInMemoryDbContext();
        var useCase = new UpdateClienteUseCase(_clienteRepoMock.Object, dbContext);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCase.ExecuteAsync(id, new UpdateClienteRequest("João", "outro@test.com")));
    }
}
