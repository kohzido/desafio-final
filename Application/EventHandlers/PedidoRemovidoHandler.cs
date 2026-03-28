using Domain.Events;
using Domain.Interfaces;

namespace Application.EventHandlers;

public class PedidoRemovidoHandler : IEventHandler<PedidoRemovido>
{
    public async Task HandleAsync(PedidoRemovido evento)
    {
        // Log da operação: pedido removido
        Console.WriteLine($"[Event] PedidoRemovido: PedidoId={evento.PedidoId}");
        await Task.CompletedTask;
    }
}
