using Domain.Entities;

namespace Domain.Interfaces;

public interface IPedidoRepository
{
    Task AddAsync(Pedido pedido);
    Task<Pedido?> GetByIdAsync(Guid id);
    Task<IEnumerable<Pedido>> GetAllAsync();
    Task UpdateAsync(Pedido pedido);
    Task DeleteAsync(Guid id);
    Task<int> CountAsync();
    Task<bool> ExisteParaClienteAsync(Guid clienteId);
    Task<bool> ExisteParaProdutoAsync(Guid produtoId);
}
