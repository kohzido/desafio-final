using Domain.Interfaces;

namespace Domain.Events;

public record PedidoRemovido(Guid PedidoId) : IDomainEvent;
