using Application.DTOs;
using Application.Validators;
using Xunit;

namespace Tests.Validators;

public class ProdutoValidatorTests
{
    private readonly CreateProdutoRequestValidator _createValidator = new();

    [Fact]
    public void CreateValidator_DevePassar_QuandoDadosValidos()
    {
        var result = _createValidator.Validate(new CreateProdutoRequest("Notebook", 3500m));
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", 10)]
    [InlineData("Produto", 0)]
    [InlineData("Produto", -1)]
    public void CreateValidator_DeveFalhar_QuandoDadosInvalidos(string nome, decimal preco)
    {
        var result = _createValidator.Validate(new CreateProdutoRequest(nome, preco));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateValidator_DeveFalhar_QuandoNomeMaiorQue200Chars()
    {
        var nomeGrande = new string('A', 201);
        var result = _createValidator.Validate(new CreateProdutoRequest(nomeGrande, 10m));
        Assert.False(result.IsValid);
    }
}
