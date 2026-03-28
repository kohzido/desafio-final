using Application.DTOs;
using Application.Exceptions;
using Domain.Interfaces;

namespace Application.UseCases.Produtos;

public class GetProdutoByIdUseCase
{
    private readonly IProdutoRepository _produtoRepo;

    public GetProdutoByIdUseCase(IProdutoRepository produtoRepo)
    {
        _produtoRepo = produtoRepo;
    }

    public async Task<ProdutoResponse> ExecuteAsync(Guid id)
    {
        var produto = await _produtoRepo.GetByIdAsync(id)
            ?? throw new NotFoundException("Produto não encontrado.");

        return new ProdutoResponse(produto.Id, produto.Nome, produto.Preco);
    }
}
