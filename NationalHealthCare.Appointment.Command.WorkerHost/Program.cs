using Microsoft.Extensions.DependencyInjection;
using NationalHealthCare.Appointment.Command.WorkerHost;
using NationalHealthCare.Appointment.Infrastructure.Services;
using NationalHealthCare.Appointment.SharedKarnel.Abstractions;
using SharedKernel.Abstractions;


var services = new ServiceCollection();

services.AddScoped<IServiceBus, RabbitMqServiceBus>();
services.AddScoped<ICommandHandler, CreateAppointmentCommandHandler>();


// Build provider
var serviceProvider = services.BuildServiceProvider();

// 3️⃣ Resolve services
var bus = serviceProvider.GetRequiredService<IServiceBus>();

var commandHandler = serviceProvider.GetRequiredService<ICommandHandler>();

await bus.StartConsumingAsync(commandHandler);




