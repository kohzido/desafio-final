using Domain.Interfaces;

namespace Domain.Events;

public record PedidoCriado(Guid PedidoId, Guid ClienteId, decimal ValorTotal) : IDomainEvent;
