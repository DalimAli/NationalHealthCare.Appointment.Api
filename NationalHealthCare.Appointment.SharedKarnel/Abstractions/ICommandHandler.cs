namespace NationalHealthCare.Appointment.SharedKarnel.Abstractions;

public interface ICommandHandler
{
    Task HandleAsync(ICommand command, CancellationToken cancellationToken = default);
}
