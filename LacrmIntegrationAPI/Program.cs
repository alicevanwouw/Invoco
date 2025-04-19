using LacrmIntegration.Application.Common;
using LacrmIntegration.Application.Interfaces;
using LacrmIntegration.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<ICallEventService, CallEventService>();
builder.Services.AddSingleton<ICallEventLogStore, InMemoryCallEventLogStore>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


// Seed dummy logs
var scope = app.Services.CreateScope();
var logStore = scope.ServiceProvider.GetRequiredService<ICallEventLogStore>();

logStore.Add(new CallEventLogEntry
{
    Timestamp = DateTime.UtcNow.AddMinutes(-30),
    CallId = Guid.NewGuid().ToString(),
    CallerName = "Alice",
    PhoneNumber = "01527306999",
    CallStart = DateTime.Today.AddHours(10),
    Status = "Success",
    ResponseMessage = "Contact created"
});

logStore.Add(new CallEventLogEntry
{
    Timestamp = DateTime.UtcNow.AddMinutes(-10),
    CallId = Guid.NewGuid().ToString(),
    CallerName = "Bob",
    PhoneNumber = "0821234567",
    CallStart = DateTime.Today.AddHours(11),
    Status = "Failed",
    ResponseMessage = "Duplicate contact"
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
