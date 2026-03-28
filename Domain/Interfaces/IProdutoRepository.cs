using Domain.Entities;

namespace Domain.Interfaces;

public interface IProdutoRepository
{
    Task AddAsync(Produto produto);
    Task<Produto?> GetByIdAsync(Guid id);
    Task<IEnumerable<Produto>> GetAllAsync();
    Task<IEnumerable<Produto>> GetByNomeAsync(string nome);
    Task UpdateAsync(Produto produto);
    Task DeleteAsync(Guid id);
    Task<int> CountAsync();
}
