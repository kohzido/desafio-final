using Application.DTOs;
using Domain.Interfaces;

namespace Application.UseCases.Produtos;

public class GetProdutoByNomeUseCase
{
    private readonly IProdutoRepository _produtoRepo;

    public GetProdutoByNomeUseCase(IProdutoRepository produtoRepo)
    {
        _produtoRepo = produtoRepo;
    }

    public async Task<IEnumerable<ProdutoResponse>> ExecuteAsync(string nome)
    {
        var produtos = await _produtoRepo.GetByNomeAsync(nome);
        return produtos.Select(p => new ProdutoResponse(p.Id, p.Nome, p.Preco));
    }
}
