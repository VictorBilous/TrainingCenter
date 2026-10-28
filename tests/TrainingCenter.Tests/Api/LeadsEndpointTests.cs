using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TrainingCenter.Api.Contracts;
using TrainingCenter.Api.Infrastructure.Mail;

namespace TrainingCenter.Tests.Api;

/// <summary>
/// Интеграционные тесты: поднимают приложение целиком (DI, валидация, JSON),
/// основной почтовый сервис «лежит» по конфигурации из appsettings.json.
/// </summary>
public class LeadsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public LeadsEndpointTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task PostLead_WhenPrimaryMailIsDown_Returns200_AndLeadGoesToBackupAddress()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/leads", new
        {
            name = "ООО Ромашка",
            phone = "+7 (495) 123-45-67",
            email = "info@romashka.ru",
            clientType = "LegalEntity",
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<LeadResponse>();
        Assert.NotNull(body);

        var mailbox = _factory.Services.GetRequiredService<InMemoryMailbox>();
        var sent = Assert.Single(mailbox.GetAll(), m => m.Message.Body.Contains(body.LeadId.ToString()));
        Assert.Equal(MailChannel.Backup, sent.Channel);
        Assert.Equal("specialist.corp@yandex.ru", sent.Message.To);
    }

    [Theory]
    [InlineData("not-an-email", "+7 999 123-45-67", "Email")]
    [InlineData("user@localhost", "+7 999 123-45-67", "Email")]
    [InlineData("ivan@example.com", "", "Phone")]
    [InlineData("ivan@example.com", "12-34", "Phone")]
    public async Task PostLead_InvalidInput_Returns400WithFieldError(string email, string phone, string invalidField)
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/leads", new
        {
            name = "Иван Петров",
            phone,
            email,
            clientType = "Individual",
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains(problem.Errors.Keys, key => key.Equals(invalidField, StringComparison.OrdinalIgnoreCase));
    }
}
