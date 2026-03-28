using Application.DTOs;
using Application.Validators;
using Xunit;

namespace Tests.Validators;

public class ClienteValidatorTests
{
    private readonly CreateClienteRequestValidator _createValidator = new();
    private readonly UpdateClienteRequestValidator _updateValidator = new();

    [Fact]
    public void CreateValidator_DevePassar_QuandoDadosValidos()
    {
        var result = _createValidator.Validate(new CreateClienteRequest("João Silva", "joao@test.com"));
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "joao@test.com")]
    [InlineData("João", "email-invalido")]
    [InlineData("João", "")]
    public void CreateValidator_DeveFalhar_QuandoDadosInvalidos(string nome, string email)
    {
        var result = _createValidator.Validate(new CreateClienteRequest(nome, email));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateValidator_DeveFalhar_QuandoNomeMaiorQue200Chars()
    {
        var nomeGrande = new string('A', 201);
        var result = _createValidator.Validate(new CreateClienteRequest(nomeGrande, "email@test.com"));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpdateValidator_DevePassar_QuandoDadosValidos()
    {
        var result = _updateValidator.Validate(new UpdateClienteRequest("João Silva", "joao@test.com"));
        Assert.True(result.IsValid);
    }
}
