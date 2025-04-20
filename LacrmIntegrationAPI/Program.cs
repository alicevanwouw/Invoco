using LacrmIntegration.Application.Common;
using LacrmIntegration.Application.Interfaces;
using LacrmIntegration.Application.Services;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<LacrmSettings>(builder.Configuration.GetSection("Lacrm"));
builder.Services.AddScoped<ICallEventService, CallEventService>();
builder.Services.AddSingleton<ICallEventLogStore, InMemoryCallEventLogStore>();
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<LacrmSettings>>().Value);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "LACRM API", Version = "v1" });
    c.ExampleFilters(); 
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<CallEventDtoExample>();

builder.Services.Configure<LacrmSettings>(
builder.Configuration.GetSection("Lacrm"));
builder.Services.AddHttpClient<ILacrmClient, LacrmClient>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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
    Endpoint = "/contacts/add",
    StatusCode = 200,
    ResponseMessage = "Contact created successfully",
    Notes = new List<string> { "Call from Fred at 2025-04-20 10:30. Number: 0823582566" }
});

logStore.Add(new CallEventLogEntry
{
    Timestamp = DateTime.UtcNow.AddDays(-1),
    Endpoint = "/contacts/add",
    StatusCode = 409,
    ResponseMessage = "Duplicate contact",
    Notes = new List<string> { "Call from Jane at 2025-04-19 12:15. Number: 0112223344" }
});

logStore.Add(new CallEventLogEntry
{
    Timestamp = DateTime.UtcNow.AddHours(-3),
    Endpoint = "/contacts/add",
    StatusCode = 409,
    ResponseMessage = "Duplicate contact",
    Notes = new List<string> { "Call from Betty at 2025-04-18 12:15. Number: 0114263446" }
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
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
