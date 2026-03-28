using Domain.Interfaces;

namespace Application.UseCases.Produtos;

public class CountProdutosUseCase
{
    private readonly IProdutoRepository _produtoRepo;

    public CountProdutosUseCase(IProdutoRepository produtoRepo)
    {
        _produtoRepo = produtoRepo;
    }

    public async Task<int> ExecuteAsync()
    {
        return await _produtoRepo.CountAsync();
    }
}
