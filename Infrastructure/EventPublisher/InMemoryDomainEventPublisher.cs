using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.EventPublisher;

public class InMemoryDomainEventPublisher : IDomainEventPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryDomainEventPublisher(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task PublishAsync(IDomainEvent domainEvent)
    {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handler = _serviceProvider.GetService(handlerType);
        if (handler != null)
            await ((dynamic)handler).HandleAsync((dynamic)domainEvent);
    }
}
