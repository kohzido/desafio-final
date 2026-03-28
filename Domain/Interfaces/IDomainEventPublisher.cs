namespace Domain.Interfaces;

public interface IDomainEventPublisher
{
    Task PublishAsync(IDomainEvent domainEvent);
}
