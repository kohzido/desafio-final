using Domain.Entities;

namespace Domain.Interfaces;

public interface IClienteRepository
{
    Task AddAsync(Cliente cliente);
    Task<Cliente?> GetByIdAsync(Guid id);
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<IEnumerable<Cliente>> GetByNomeAsync(string nome);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(Guid id);
    Task<int> CountAsync();
    Task<bool> ExisteEmailAsync(string email, Guid? excludeId = null);
}
