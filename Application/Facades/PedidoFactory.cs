using Domain.Entities;

namespace Application.Facades;

// NOTA: Pedido é uma class mutável (não record), pois Atualizar modifica propriedades in-place.
public class PedidoFactory
{
    public Pedido Criar(Guid clienteId, List<Produto> produtos)
    {
        return new Pedido
        {
            Id = Guid.NewGuid(),
            ClienteId = clienteId,
            Produtos = produtos,
            ValorTotal = produtos.Sum(p => p.Preco)
        };
    }

    public Pedido Atualizar(Pedido pedido, List<Produto> produtos)
    {
        // Clear + AddRange preserva o objeto de coleção rastreado pelo EF Core.
        // Substituir a referência (pedido.Produtos = produtos) abandona a coleção
        // rastreada e quebra o change tracking do many-to-many.
        pedido.Produtos.Clear();
        pedido.Produtos.AddRange(produtos);
        pedido.ValorTotal = produtos.Sum(p => p.Preco);
        return pedido;
    }
}
