// NOTA: O sufixo "Adapter" é mantido intencionalmente para evidenciar o uso do Design Pattern Adapter.
// Em projetos convencionais, este sufixo costuma ser omitido (ex: ProdutoRepository).
// Aqui é preservado como sinal arquitetural explícito: esta classe adapta a interface do Domain
// para a implementação concreta com Entity Framework Core / PostgreSQL.

using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ProdutoRepositoryAdapter : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepositoryAdapter(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Produto produto)
    {
        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();
    }

    public async Task<Produto?> GetByIdAsync(Guid id)
    {
        return await _context.Produtos.FindAsync(id);
    }

    public async Task<IEnumerable<Produto>> GetAllAsync()
    {
        return await _context.Produtos.ToListAsync();
    }

    public async Task<IEnumerable<Produto>> GetByNomeAsync(string nome)
    {
        return await _context.Produtos
            .Where(p => p.Nome.ToLower().Contains(nome.ToLower()))
            .ToListAsync();
    }

    public async Task UpdateAsync(Produto produto)
    {
        var tracked = _context.ChangeTracker.Entries<Produto>()
            .FirstOrDefault(e => e.Entity.Id == produto.Id);
        if (tracked != null)
            tracked.State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto != null)
        {
            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> CountAsync()
    {
        return await _context.Produtos.CountAsync();
    }
}
