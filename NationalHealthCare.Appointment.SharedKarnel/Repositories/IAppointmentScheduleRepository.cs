using NationalHealthCare.Appointment.SharedKarnel.Aggregates;

namespace NationalHealthCare.Appointment.SharedKarnel.Repositories;

public interface IAppointmentScheduleRepository
{
    Task<AppointmentSchedule> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AppointmentSchedule> GetByDateAsync(DateTime scheduleDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentSchedule>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(AppointmentSchedule schedule, CancellationToken cancellationToken = default);
    Task UpdateAsync(AppointmentSchedule schedule, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
