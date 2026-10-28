using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.Domain;
using TrainingCenter.Api.Infrastructure.Data;

namespace TrainingCenter.Api.Controllers;

[ApiController]
[Route("api/courses")]
[Produces("application/json")]
public sealed class CoursesController : ControllerBase
{
    private readonly ICourseRepository _courses;

    public CoursesController(ICourseRepository courses) => _courses = courses;

    /// <summary>Каталог курсов (чтобы видеть даты старта при проверке скидок).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Course>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<Course>>> GetAll(CancellationToken ct) =>
        Ok(await _courses.GetAllAsync(ct));
}
