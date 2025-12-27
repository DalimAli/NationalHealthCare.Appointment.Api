using NationalHealthCare.Appointment.SharedKarnel.Aggregates;
using NationalHealthCare.Appointment.SharedKarnel.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalHealthCare.Appointment.Infrastructure.Services
{
    public class AppointmentScheduleRepository : IAppointmentScheduleRepository
    {
        public async Task AddAsync(AppointmentSchedule schedule, CancellationToken cancellationToken = default)
        {

            await Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppointmentSchedule>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AppointmentSchedule> GetByDateAsync(DateTime scheduleDate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AppointmentSchedule> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(AppointmentSchedule schedule, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
