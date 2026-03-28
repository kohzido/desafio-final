using Application.DTOs;
using Application.UseCases.Produtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("produtos")]
[Produces("application/json")]
public class ProdutoController : ControllerBase
{
    private readonly CreateProdutoUseCase _createUseCase;
    private readonly GetAllProdutosUseCase _getAllUseCase;
    private readonly GetProdutoByIdUseCase _getByIdUseCase;
    private readonly GetProdutoByNomeUseCase _getByNomeUseCase;
    private readonly UpdateProdutoUseCase _updateUseCase;
    private readonly DeleteProdutoUseCase _deleteUseCase;
    private readonly CountProdutosUseCase _countUseCase;

    public ProdutoController(
        CreateProdutoUseCase createUseCase,
        GetAllProdutosUseCase getAllUseCase,
        GetProdutoByIdUseCase getByIdUseCase,
        GetProdutoByNomeUseCase getByNomeUseCase,
        UpdateProdutoUseCase updateUseCase,
        DeleteProdutoUseCase deleteUseCase,
        CountProdutosUseCase countUseCase)
    {
        _createUseCase = createUseCase;
        _getAllUseCase = getAllUseCase;
        _getByIdUseCase = getByIdUseCase;
        _getByNomeUseCase = getByNomeUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
        _countUseCase = countUseCase;
    }

    /// <summary>Cria um novo produto</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProdutoRequest request)
    {
        var result = await _createUseCase.ExecuteAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Lista todos os produtos</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _getAllUseCase.ExecuteAsync();
        return Ok(result);
    }

    /// <summary>Busca produto por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _getByIdUseCase.ExecuteAsync(id);
        return Ok(result);
    }

    /// <summary>Busca produtos por nome (busca parcial, case-insensitive)</summary>
    [HttpGet("nome/{nome}")]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByNome(string nome)
    {
        var result = await _getByNomeUseCase.ExecuteAsync(nome);
        return Ok(result);
    }

    /// <summary>Atualiza um produto existente</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProdutoRequest request)
    {
        var result = await _updateUseCase.ExecuteAsync(id, request);
        return Ok(result);
    }

    /// <summary>Remove um produto</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _deleteUseCase.ExecuteAsync(id);
        return NoContent();
    }

    /// <summary>Retorna a contagem total de produtos</summary>
    [HttpGet("count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> Count()
    {
        var result = await _countUseCase.ExecuteAsync();
        return Ok(result);
    }
}
