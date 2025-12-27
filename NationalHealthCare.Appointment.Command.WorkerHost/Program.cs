using Microsoft.Extensions.DependencyInjection;
using NationalHealthCare.Appointment.Command.WorkerHost;
using NationalHealthCare.Appointment.Infrastructure.Services;
using NationalHealthCare.Appointment.SharedKarnel;
using NationalHealthCare.Appointment.SharedKarnel.Abstractions;
using SharedKernel.Abstractions;


var services = new ServiceCollection();

services.AddScoped<IServiceBus, RabbitMqServiceBus>();
services.AddScoped<ICommandHandler<CreateAppointmentCommand>, CreateAppointmentCommandHandler>();


// Build provider
var serviceProvider = services.BuildServiceProvider();

// 3️⃣ Resolve services
var bus = serviceProvider.GetRequiredService<IServiceBus>();

var commandHandler = serviceProvider.GetRequiredService<ICommandHandler<CreateAppointmentCommand>>();

await bus.StartConsumingAsync(commandHandler);




