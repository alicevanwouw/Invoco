using LacrmIntegration.Application.Common;
using LacrmIntegration.Application.Interfaces;
using LacrmIntegration.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<ICallEventService, CallEventService>();
builder.Services.AddSingleton<ICallEventLogStore, InMemoryCallEventLogStore>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LACRM Integration API");
    });
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
    CallStart = DateTime.Now.AddMinutes(-30),
    Status = "Success",
    ResponseMessage = "Contact created"
});

logStore.Add(new CallEventLogEntry
{
    Timestamp = DateTime.UtcNow.AddDays(-1),
    CallId = Guid.NewGuid().ToString(),
    CallerName = "Bob",
    PhoneNumber = "0821234567",
    CallStart = DateTime.Now.AddDays(-1),
    Status = "Failed",
    ResponseMessage = "Duplicate contact"
});

logStore.Add(new CallEventLogEntry
{
    Timestamp = DateTime.UtcNow.AddHours(-3),
    CallId = Guid.NewGuid().ToString(),
    CallerName = "Fred",
    PhoneNumber = "0823582566",
    CallStart = DateTime.Now.AddHours(-3),
    Status = "Success",
    ResponseMessage = "Contact Created"
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Alert API");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
