using NationalHealthCare.Appointment.SharedKarnel.Abstractions;

namespace SharedKernel.Abstractions;

public interface IServiceBus
{
    Task PublishAsync(ICommand message, CancellationToken cancellationToken = default);

    Task StartConsumingAsync(ICommandHandler commandHandler, CancellationToken cancellationToken = default);
}
