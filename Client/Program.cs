using Client;
using MassTransit;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddOpenTelemetry().WithTracing(x =>
{
    x.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("myClient"))
       .AddSource("MassTransit")
    .AddZipkinExporter(config => config.Endpoint = new Uri("http://localhost:9411/api/v2/spans"))
    .AddAspNetCoreInstrumentation();
    //.Build();
}); 
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmployeeConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
        cfg.ReceiveEndpoint("employee", e =>
            {
                e.ConfigureConsumer<EmployeeConsumer>(context);
            });
        cfg.ConfigureEndpoints(context);
     
    });
});
builder.Services.AddMassTransitHostedService();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
