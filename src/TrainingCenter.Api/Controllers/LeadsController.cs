using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.Contracts;
using TrainingCenter.Api.Services.Leads;

namespace TrainingCenter.Api.Controllers;

/// <summary>
/// Тонкий контроллер: валидацию делает [ApiController] (400 до входа в action),
/// доставку и fallback — сервисный слой <see cref="ILeadService"/>.
/// </summary>
[ApiController]
[Route("api/leads")]
[Produces("application/json")]
public sealed class LeadsController : ControllerBase
{
    private readonly ILeadService _leadService;

    public LeadsController(ILeadService leadService) => _leadService = leadService;

    /// <summary>Отправить заявку с формы «Обратный звонок».</summary>
    /// <remarks>
    /// Если основной почтовый сервис недоступен, заявка уходит на резервный адрес
    /// (юр. лица — specialist.corp@yandex.ru, физ. лица — specialist.ufl@yandex.ru),
    /// а клиент всё равно получает 200 OK.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(LeadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LeadResponse>> Create([FromBody] LeadRequest request, CancellationToken ct)
    {
        var lead = await _leadService.SubmitAsync(request, ct);

        return Ok(new LeadResponse(lead.Id, "Спасибо! Мы перезвоним вам в ближайшее время."));
    }
}
