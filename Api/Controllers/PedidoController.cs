using Application.DTOs;
using Application.UseCases.Pedidos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("pedidos")]
[Produces("application/json")]
public class PedidoController : ControllerBase
{
    private readonly CreatePedidoUseCase _createUseCase;
    private readonly GetAllPedidosUseCase _getAllUseCase;
    private readonly GetPedidoByIdUseCase _getByIdUseCase;
    private readonly UpdatePedidoUseCase _updateUseCase;
    private readonly DeletePedidoUseCase _deleteUseCase;
    private readonly CountPedidosUseCase _countUseCase;

    public PedidoController(
        CreatePedidoUseCase createUseCase,
        GetAllPedidosUseCase getAllUseCase,
        GetPedidoByIdUseCase getByIdUseCase,
        UpdatePedidoUseCase updateUseCase,
        DeletePedidoUseCase deleteUseCase,
        CountPedidosUseCase countUseCase)
    {
        _createUseCase = createUseCase;
        _getAllUseCase = getAllUseCase;
        _getByIdUseCase = getByIdUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
        _countUseCase = countUseCase;
    }

    /// <summary>Cria um novo pedido</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreatePedidoRequest request)
    {
        var result = await _createUseCase.ExecuteAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Lista todos os pedidos</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PedidoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _getAllUseCase.ExecuteAsync();
        return Ok(result);
    }

    /// <summary>Busca pedido por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _getByIdUseCase.ExecuteAsync(id);
        return Ok(result);
    }

    /// <summary>Atualiza um pedido existente (substitui lista de produtos)</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePedidoRequest request)
    {
        var result = await _updateUseCase.ExecuteAsync(id, request);
        return Ok(result);
    }

    /// <summary>Remove um pedido</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _deleteUseCase.ExecuteAsync(id);
        return NoContent();
    }

    /// <summary>Retorna a contagem total de pedidos</summary>
    [HttpGet("count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> Count()
    {
        var result = await _countUseCase.ExecuteAsync();
        return Ok(result);
    }
}
