using NationalHealthCare.Appointment.SharedKarnel.Abstractions;

namespace SharedKernel.Abstractions;

public interface IServiceBus
{
    Task PublishAsync(ICommand message, CancellationToken cancellationToken = default);
    
    Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : class;
    
    Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
}
