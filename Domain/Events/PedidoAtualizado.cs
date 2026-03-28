using Domain.Interfaces;

namespace Domain.Events;

public record PedidoAtualizado(Guid PedidoId, decimal NovoValorTotal) : IDomainEvent;
