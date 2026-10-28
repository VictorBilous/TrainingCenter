using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.Contracts;
using TrainingCenter.Api.Services.Pricing;

namespace TrainingCenter.Api.Controllers;

[ApiController]
[Route("api/cart")]
[Produces("application/json")]
public sealed class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService) => _cartService = cartService;

    /// <summary>Рассчитать стоимость корзины с учётом скидок.</summary>
    /// <remarks>
    /// На каждый курс применяется одна скидка — максимальная из доступных:
    /// «Горящая группа» (до старта менее 5 дней) — 20%, «Выпускник» — 30%.
    /// </remarks>
    [HttpPost("price")]
    [ProducesResponseType(typeof(CartPrice), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartPrice>> Calculate([FromBody] CartPriceRequest request, CancellationToken ct)
    {
        var price = await _cartService.CalculateAsync(request.UserId!.Value, request.CourseIds, ct);

        return price is null
            ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Пользователь или курс не найден")
            : Ok(price);
    }
}
