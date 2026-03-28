using Domain.Events;
using Domain.Interfaces;

namespace Application.EventHandlers;

public class PedidoCriadoHandler : IEventHandler<PedidoCriado>
{
    public async Task HandleAsync(PedidoCriado evento)
    {
        // Log da operação: pedido criado
        Console.WriteLine($"[Event] PedidoCriado: PedidoId={evento.PedidoId}, ClienteId={evento.ClienteId}, ValorTotal={evento.ValorTotal}");
        await Task.CompletedTask;
    }
}
