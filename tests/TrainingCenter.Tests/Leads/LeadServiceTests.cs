using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using TrainingCenter.Api.Contracts;
using TrainingCenter.Api.Domain;
using TrainingCenter.Api.Infrastructure.Mail;
using TrainingCenter.Api.Services.Leads;

namespace TrainingCenter.Tests.Leads;

public class LeadServiceTests
{
    private const string PrimaryRecipient = "leads@training-center.local";
    private const string IndividualBackup = "specialist.ufl@yandex.ru";
    private const string LegalEntityBackup = "specialist.corp@yandex.ru";

    private readonly Mock<IMailService> _primaryMail = new();
    private readonly Mock<IBackupMailService> _backupMail = new();

    private LeadService CreateService(TimeSpan? primaryTimeout = null) => new(
        _primaryMail.Object,
        _backupMail.Object,
        Options.Create(new LeadRoutingOptions
        {
            PrimaryRecipient = PrimaryRecipient,
            IndividualBackupRecipient = IndividualBackup,
            LegalEntityBackupRecipient = LegalEntityBackup,
            PrimaryTimeout = primaryTimeout ?? TimeSpan.FromSeconds(5),
        }),
        TimeProvider.System,
        NullLogger<LeadService>.Instance);

    private static LeadRequest CreateRequest(ClientType clientType) => new()
    {
        Name = "Иван Петров",
        Phone = "+7 999 123-45-67",
        Email = "ivan@example.com",
        ClientType = clientType,
    };

    [Fact]
    public async Task SubmitAsync_PrimaryWorks_BackupIsNotUsed()
    {
        await CreateService().SubmitAsync(CreateRequest(ClientType.Individual), CancellationToken.None);

        _primaryMail.Verify(
            s => s.SendAsync(It.Is<EmailMessage>(m => m.To == PrimaryRecipient), It.IsAny<CancellationToken>()),
            Times.Once);
        _backupMail.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(ClientType.LegalEntity, LegalEntityBackup)]
    [InlineData(ClientType.Individual, IndividualBackup)]
    public async Task SubmitAsync_PrimaryThrows_LeadIsRedirectedToBackupByClientType(
        ClientType clientType, string expectedRecipient)
    {
        _primaryMail
            .Setup(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new MailServiceUnavailableException("SMTP down"));

        var lead = await CreateService().SubmitAsync(CreateRequest(clientType), CancellationToken.None);

        _backupMail.Verify(
            s => s.SendAsync(
                It.Is<EmailMessage>(m => m.To == expectedRecipient && m.Body.Contains(lead.Id.ToString())),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_PrimaryHangs_FallsBackAfterTimeout()
    {
        // Сервис не падает, а «висит» до отмены токена.
        _primaryMail
            .Setup(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Returns((EmailMessage _, CancellationToken token) => Task.Delay(Timeout.Infinite, token));

        await CreateService(primaryTimeout: TimeSpan.FromMilliseconds(50))
            .SubmitAsync(CreateRequest(ClientType.Individual), CancellationToken.None);

        _backupMail.Verify(
            s => s.SendAsync(It.Is<EmailMessage>(m => m.To == IndividualBackup), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_ClientCancelledRequest_DoesNotFallBack()
    {
        using var cts = new CancellationTokenSource();
        _primaryMail
            .Setup(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Returns((EmailMessage _, CancellationToken token) =>
            {
                cts.Cancel(); // клиент закрыл страницу, пока шла отправка
                return Task.FromCanceled(token);
            });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => CreateService().SubmitAsync(CreateRequest(ClientType.Individual), cts.Token));

        _backupMail.VerifyNoOtherCalls();
    }
}
