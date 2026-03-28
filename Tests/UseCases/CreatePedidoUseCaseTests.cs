using Application.DTOs;
using Application.Exceptions;
using Application.Facades;
using Application.UseCases.Pedidos;
using Application.Validators;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using Moq;
using Tests.Helpers;
using Xunit;

namespace Tests.UseCases;

public class CreatePedidoUseCaseTests
{
    private readonly Mock<PedidoFacade> _facadeMock;
    private readonly Mock<IDomainEventPublisher> _publisherMock = new();
    private readonly IValidator<CreatePedidoRequest> _validator = new CreatePedidoRequestValidator();

    public CreatePedidoUseCaseTests()
    {
        var validatorSvc = new Mock<PedidoValidatorService>(
            new Mock<IClienteRepository>().Object,
            new Mock<IProdutoRepository>().Object,
            new Mock<IPedidoRepository>().Object);
        var factory = new PedidoFactory();
        var pedidoRepo = new Mock<IPedidoRepository>();
        _facadeMock = new Mock<PedidoFacade>(validatorSvc.Object, factory, pedidoRepo.Object);
    }

    [Fact]
    public async Task ExecuteAsync_DeveCriarPedido_QuandoDadosValidos()
    {
        var clienteId = Guid.NewGuid();
        var produtoId = Guid.NewGuid();
        var pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            ClienteId = clienteId,
            Produtos = new List<Produto> { new Produto(produtoId, "Produto A", 50m) },
            ValorTotal = 50m
        };

        _facadeMock.Setup(f => f.CriarPedidoAsync(clienteId, It.IsAny<List<Guid>>()))
            .ReturnsAsync(pedido);
        _publisherMock.Setup(p => p.PublishAsync(It.IsAny<Domain.Interfaces.IDomainEvent>()))
            .Returns(Task.CompletedTask);

        using var dbContext = MockDbContextHelper.CreateInMemoryDbContext();
        var useCase = new CreatePedidoUseCase(_facadeMock.Object, _publisherMock.Object, _validator, dbContext);

        var request = new CreatePedidoRequest(clienteId, new List<Guid> { produtoId });
        var result = await useCase.ExecuteAsync(request);

        Assert.NotNull(result);
        Assert.Equal(clienteId, result.ClienteId);
        Assert.Equal(50m, result.ValorTotal);
        _publisherMock.Verify(p => p.PublishAsync(It.IsAny<Domain.Interfaces.IDomainEvent>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarValidationException_QuandoProdutoIdsVazio()
    {
        using var dbContext = MockDbContextHelper.CreateInMemoryDbContext();
        var useCase = new CreatePedidoUseCase(_facadeMock.Object, _publisherMock.Object, _validator, dbContext);

        var request = new CreatePedidoRequest(Guid.NewGuid(), new List<Guid>());

        await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarNotFoundException_QuandoClienteNaoEncontrado()
    {
        var clienteId = Guid.NewGuid();
        _facadeMock.Setup(f => f.CriarPedidoAsync(clienteId, It.IsAny<List<Guid>>()))
            .ThrowsAsync(new NotFoundException("Cliente não encontrado."));

        using var dbContext = MockDbContextHelper.CreateInMemoryDbContext();
        var useCase = new CreatePedidoUseCase(_facadeMock.Object, _publisherMock.Object, _validator, dbContext);

        var request = new CreatePedidoRequest(clienteId, new List<Guid> { Guid.NewGuid() });

        await Assert.ThrowsAsync<NotFoundException>(() => useCase.ExecuteAsync(request));
    }
}
