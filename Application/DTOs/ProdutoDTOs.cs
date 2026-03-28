namespace Application.DTOs;

public record CreateProdutoRequest(string Nome, decimal Preco);

public record UpdateProdutoRequest(string Nome, decimal Preco);

public record ProdutoResponse(Guid Id, string Nome, decimal Preco);
