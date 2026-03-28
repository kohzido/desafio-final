namespace Application.DTOs;

public record CreatePedidoRequest(Guid ClienteId, List<Guid> ProdutoIds);

public record UpdatePedidoRequest(List<Guid> ProdutoIds);

public record PedidoResponse(Guid Id, Guid ClienteId, List<ProdutoResponse> Produtos, decimal ValorTotal);
