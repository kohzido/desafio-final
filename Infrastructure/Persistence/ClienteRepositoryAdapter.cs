// NOTA: O sufixo "Adapter" é mantido intencionalmente para evidenciar o uso do Design Pattern Adapter.
// Em projetos convencionais, este sufixo costuma ser omitido (ex: ClienteRepository).
// Aqui é preservado como sinal arquitetural explícito: esta classe adapta a interface do Domain
// para a implementação concreta com Entity Framework Core / PostgreSQL.

using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ClienteRepositoryAdapter : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepositoryAdapter(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Cliente cliente)
    {
        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task<Cliente?> GetByIdAsync(Guid id)
    {
        return await _context.Clientes.FindAsync(id);
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _context.Clientes.ToListAsync();
    }

    public async Task<IEnumerable<Cliente>> GetByNomeAsync(string nome)
    {
        return await _context.Clientes
            .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
            .ToListAsync();
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        var tracked = _context.ChangeTracker.Entries<Cliente>()
            .FirstOrDefault(e => e.Entity.Id == cliente.Id);
        if (tracked != null)
            tracked.State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente != null)
        {
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> CountAsync()
    {
        return await _context.Clientes.CountAsync();
    }

    public async Task<bool> ExisteEmailAsync(string email, Guid? excludeId = null)
    {
        var query = _context.Clientes.Where(c => c.Email == email);
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
        return await query.AnyAsync();
    }
}
