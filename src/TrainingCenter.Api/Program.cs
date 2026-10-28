using System.Reflection;
using System.Text.Json.Serialization;
using TrainingCenter.Api;
using TrainingCenter.Api.Infrastructure.Mail;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddPricing();
builder.Services.AddLeads(builder.Configuration);

var app = builder.Build();

// Необработанные исключения превращаются в 500 ProblemDetails без стека вызовов.
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Только для демонстрации: какие письма «ушли», через какой канал и на какой адрес.
    app.MapGet("/api/debug/mailbox", (InMemoryMailbox mailbox) => mailbox.GetAll())
        .WithTags("Debug");
}

app.MapControllers();

app.Run();

// Нужен интеграционным тестам: WebApplicationFactory<Program>.
public partial class Program { }
