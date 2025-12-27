namespace NationalHealthCare.Appointment.SharedKarnel.Abstractions;

public interface ICommandHandler<T> where T : ICommand
{
    Task HandleAsync(T command, CancellationToken cancellationToken = default);
}
