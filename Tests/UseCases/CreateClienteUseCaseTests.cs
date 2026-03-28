using Application.DTOs;
using Application.Exceptions;
using Application.UseCases.Clientes;
using Application.Validators;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Tests.Helpers;
using Xunit;

namespace Tests.UseCases;

public class CreateClienteUseCaseTests
{
    private readonly Mock<IClienteRepository> _clienteRepoMock = new();

    [Fact]
    public async Task ExecuteAsync_DeveRetornarClienteResponse_QuandoDadosValidos()
    {
        _clienteRepoMock.Setup(r => r.ExisteEmailAsync("email@test.com", null)).ReturnsAsync(false);
        _clienteRepoMock.Setup(r => r.AddAsync(It.IsAny<Cliente>())).Returns(Task.CompletedTask);

        using var dbContext = MockDbContextHelper.CreateInMemoryDbContext();
        var useCase = new CreateClienteUseCase(_clienteRepoMock.Object, dbContext, new CreateClienteRequestValidator());

        var result = await useCase.ExecuteAsync(new CreateClienteRequest("João", "email@test.com"));

        Assert.NotNull(result);
        Assert.Equal("João", result.Nome);
        Assert.Equal("email@test.com", result.Email);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarBusinessRuleException_QuandoEmailJaCadastrado()
    {
        _clienteRepoMock.Setup(r => r.ExisteEmailAsync("email@test.com", null)).ReturnsAsync(true);

        using var dbContext = MockDbContextHelper.CreateInMemoryDbContext();
        var useCase = new CreateClienteUseCase(_clienteRepoMock.Object, dbContext, new CreateClienteRequestValidator());

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCase.ExecuteAsync(new CreateClienteRequest("João", "email@test.com")));
    }
}
