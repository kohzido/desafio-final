namespace Domain.Interfaces;

public interface IEventHandler<T> where T : IDomainEvent
{
    Task HandleAsync(T domainEvent);
}
