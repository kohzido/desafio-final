using Application.DTOs;
using Application.Exceptions;
using Application.UseCases.Clientes;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("clientes")]
[Produces("application/json")]
public class ClienteController : ControllerBase
{
    private readonly CreateClienteUseCase _createUseCase;
    private readonly GetAllClientesUseCase _getAllUseCase;
    private readonly GetClienteByIdUseCase _getByIdUseCase;
    private readonly GetClienteByNomeUseCase _getByNomeUseCase;
    private readonly UpdateClienteUseCase _updateUseCase;
    private readonly DeleteClienteUseCase _deleteUseCase;
    private readonly CountClientesUseCase _countUseCase;

    public ClienteController(
        CreateClienteUseCase createUseCase,
        GetAllClientesUseCase getAllUseCase,
        GetClienteByIdUseCase getByIdUseCase,
        GetClienteByNomeUseCase getByNomeUseCase,
        UpdateClienteUseCase updateUseCase,
        DeleteClienteUseCase deleteUseCase,
        CountClientesUseCase countUseCase)
    {
        _createUseCase = createUseCase;
        _getAllUseCase = getAllUseCase;
        _getByIdUseCase = getByIdUseCase;
        _getByNomeUseCase = getByNomeUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
        _countUseCase = countUseCase;
    }

    /// <summary>Cria um novo cliente</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateClienteRequest request)
    {
        var result = await _createUseCase.ExecuteAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Lista todos os clientes</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _getAllUseCase.ExecuteAsync();
        return Ok(result);
    }

    /// <summary>Busca cliente por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _getByIdUseCase.ExecuteAsync(id);
        return Ok(result);
    }

    /// <summary>Busca clientes por nome (busca parcial, case-insensitive)</summary>
    [HttpGet("nome/{nome}")]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByNome(string nome)
    {
        var result = await _getByNomeUseCase.ExecuteAsync(nome);
        return Ok(result);
    }

    /// <summary>Atualiza um cliente existente</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClienteRequest request)
    {
        var result = await _updateUseCase.ExecuteAsync(id, request);
        return Ok(result);
    }

    /// <summary>Remove um cliente</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _deleteUseCase.ExecuteAsync(id);
        return NoContent();
    }

    /// <summary>Retorna a contagem total de clientes</summary>
    [HttpGet("count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> Count()
    {
        var result = await _countUseCase.ExecuteAsync();
        return Ok(result);
    }
}
