using Application.DTOs;
using Domain.Interfaces;

namespace Application.UseCases.Produtos;

public class GetAllProdutosUseCase
{
    private readonly IProdutoRepository _produtoRepo;

    public GetAllProdutosUseCase(IProdutoRepository produtoRepo)
    {
        _produtoRepo = produtoRepo;
    }

    public async Task<IEnumerable<ProdutoResponse>> ExecuteAsync()
    {
        var produtos = await _produtoRepo.GetAllAsync();
        return produtos.Select(p => new ProdutoResponse(p.Id, p.Nome, p.Preco));
    }
}
