using Common;
using MassTransit;

namespace Client
{
    public class EmployeeConsumer : IConsumer<Employee>
    {
        private readonly ILogger<EmployeeConsumer> logger;

        public EmployeeConsumer(ILogger<EmployeeConsumer> logger)
        {
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<Employee> context)
        {
            var message = context.Message;
            var activity = System.Diagnostics.Activity.Current;
            if (activity != null)
            {
                // Add custom tags like the message body
                activity.AddTag("message.body", System.Text.Json.JsonSerializer.Serialize(message));
            }

            Console.WriteLine($"Received message: {message.name}");
            logger.LogInformation("Received message:{message}", message);

            // Business logic here
            await Task.CompletedTask;
        }
    }
}
