// NOTA: O sufixo "Adapter" é mantido intencionalmente para evidenciar o uso do Design Pattern Adapter.
// Em projetos convencionais, este sufixo costuma ser omitido (ex: PedidoRepository).
// Aqui é preservado como sinal arquitetural explícito: esta classe adapta a interface do Domain
// para a implementação concreta com Entity Framework Core / PostgreSQL.

using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class PedidoRepositoryAdapter : IPedidoRepository
{
    private readonly AppDbContext _context;

    public PedidoRepositoryAdapter(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Pedido pedido)
    {
        await _context.Pedidos.AddAsync(pedido);
        await _context.SaveChangesAsync();
    }

    public async Task<Pedido?> GetByIdAsync(Guid id)
    {
        return await _context.Pedidos
            .Include(p => p.Produtos)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Pedido>> GetAllAsync()
    {
        return await _context.Pedidos
            .Include(p => p.Produtos)
            .ToListAsync();
    }

    public async Task UpdateAsync(Pedido pedido)
    {
        _context.Pedidos.Update(pedido);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido != null)
        {
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> CountAsync()
    {
        return await _context.Pedidos.CountAsync();
    }

    public async Task<bool> ExisteParaClienteAsync(Guid clienteId)
    {
        return await _context.Pedidos.AnyAsync(p => p.ClienteId == clienteId);
    }

    public async Task<bool> ExisteParaProdutoAsync(Guid produtoId)
    {
        return await _context.Pedidos
            .AnyAsync(p => p.Produtos.Any(prod => prod.Id == produtoId));
    }
}
