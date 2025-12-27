using NationalHealthCare.Appointment.SharedKarnel.Abstractions;

namespace SharedKernel.Abstractions;

public interface IServiceBus
{
    Task PublishAsync(ICommand message, CancellationToken cancellationToken = default);

    Task StartConsumingAsync<TResponse>(ICommandHandler<TResponse> commandHandler, CancellationToken cancellationToken = default) where TResponse : ICommand;
}
