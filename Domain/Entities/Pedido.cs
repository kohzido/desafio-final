namespace Domain.Entities;

// Pedido é uma class mutável (não record), pois PedidoFactory.Atualizar modifica propriedades in-place.
public class Pedido
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public List<Produto> Produtos { get; set; } = new();
    public decimal ValorTotal { get; set; }
}
