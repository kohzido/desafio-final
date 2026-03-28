using Application.DTOs;
using Application.Validators;
using Xunit;

namespace Tests.Validators;

public class PedidoValidatorTests
{
    private readonly CreatePedidoRequestValidator _createValidator = new();
    private readonly UpdatePedidoRequestValidator _updateValidator = new();

    [Fact]
    public void CreateValidator_DevePassar_QuandoDadosValidos()
    {
        var result = _createValidator.Validate(
            new CreatePedidoRequest(Guid.NewGuid(), new List<Guid> { Guid.NewGuid() }));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateValidator_DevePassar_QuandoClienteIdEhGuidZero()
    {
        // Guid.Empty é um ID inexistente, não um campo estruturalmente inválido.
        // A verificação de existência é responsabilidade do PedidoValidatorService (→ 404).
        var result = _createValidator.Validate(
            new CreatePedidoRequest(Guid.Empty, new List<Guid> { Guid.NewGuid() }));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateValidator_DeveFalhar_QuandoProdutoIdsVazio()
    {
        var result = _createValidator.Validate(
            new CreatePedidoRequest(Guid.NewGuid(), new List<Guid>()));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpdateValidator_DeveFalhar_QuandoProdutoIdsVazio()
    {
        var result = _updateValidator.Validate(new UpdatePedidoRequest(new List<Guid>()));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpdateValidator_DevePassar_QuandoDadosValidos()
    {
        var result = _updateValidator.Validate(
            new UpdatePedidoRequest(new List<Guid> { Guid.NewGuid() }));
        Assert.True(result.IsValid);
    }
}
