using Domain.Events;
using Domain.Interfaces;

namespace Application.EventHandlers;

public class PedidoAtualizadoHandler : IEventHandler<PedidoAtualizado>
{
    public async Task HandleAsync(PedidoAtualizado evento)
    {
        // Log da operação: pedido atualizado
        Console.WriteLine($"[Event] PedidoAtualizado: PedidoId={evento.PedidoId}, NovoValorTotal={evento.NovoValorTotal}");
        await Task.CompletedTask;
    }
}
