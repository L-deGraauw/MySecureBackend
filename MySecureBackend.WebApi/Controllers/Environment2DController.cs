using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;

namespace MySecureBackend.WebApi.Controllers;
//This is a change to test a Git Commit
// another test commit
//[Authorize] // TODO: Uncomment when authentication is enabled
[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class Environment2DController : ControllerBase
{
    private readonly IEnvironment2DRepository _environment2DRepository;
    private readonly IAuthenticationService _authenticationService;

    public Environment2DController(IEnvironment2DRepository environment2DRepository, IAuthenticationService authenticationService)
    {
        _environment2DRepository = environment2DRepository;
        _authenticationService = authenticationService;
    }

    private string? GetUserId()
    {
        // Try authenticated user first, fall back to header for development
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            userId = Request.Headers["X-User-Id"].FirstOrDefault();
        return userId;
    }

    [HttpGet(Name = "GetEnvironments")]
    public async Task<ActionResult> GetAsync()
    {
        var environments = await _environment2DRepository.SelectAsync();
        var result = environments.Select(e => new { e.EnvironmentId, e.Name, e.OwnerUserId });
        return Ok(result);
    }

    [HttpGet("{id}", Name = "GetEnvironmentById")]
    public async Task<ActionResult<Environment2D>> GetByIdAsync(Guid EnvironmentId)
    {
        var environment = await _environment2DRepository.SelectAsync(EnvironmentId);

        if (environment == null)
            return NotFound(new ProblemDetails { Detail = $"Environment {EnvironmentId} not found" });

        return Ok(environment);
    }

    [HttpPost(Name = "CreateEnvironment")]
    public async Task<ActionResult<Environment2D>> CreateAsync(Environment2D environment)
    {
        var userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ProblemDetails { Detail = "User identification is required. Provide an X-User-Id header or authenticate." });

        var count = await _environment2DRepository.CountByOwnerAsync(userId);
        if (count >= 5)
            return Conflict(new ProblemDetails { Detail = "You have reached the maximum of 5 environments" });

        var nameExists = await _environment2DRepository.ExistsByOwnerAndNameAsync(userId, environment.Name);
        if (nameExists)
            return Conflict(new ProblemDetails { Detail = $"You already have an environment named '{environment.Name}'" });

        environment.EnvironmentId = Guid.NewGuid();
        environment.OwnerUserId = userId;

        await _environment2DRepository.InsertAsync(environment);

        return CreatedAtRoute("GetEnvironmentById", new { id = environment.EnvironmentId }, environment);
    }

    [HttpDelete("{id}", Name = "DeleteEnvironment")]
    public async Task<ActionResult> DeleteAsync(Guid EnvironmentId)
    {
        var userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ProblemDetails { Detail = "User identification is required. Provide an X-User-Id header or authenticate." });

        var environment = await _environment2DRepository.SelectAsync(EnvironmentId);

        if (environment == null || environment.OwnerUserId != userId)
            return NotFound(new ProblemDetails { Detail = $"Environment {EnvironmentId} not found" });

        await _environment2DRepository.DeleteAsync(EnvironmentId);

        return Ok();
    }

    [HttpPut("{id}", Name = "UpdateEnvironment")]
    public async Task<ActionResult<Environment2D>> UpdateAsync(Guid id, Environment2D environment)
    {
        var existingEnvironment = await _environment2DRepository.SelectAsync(id);

        if (existingEnvironment == null)
            return NotFound(new ProblemDetails { Detail = $"Environment {id} not found" });

        if (environment.EnvironmentId != id)
            return Conflict(new ProblemDetails { Detail = "The id of the Environment in the route does not match the id of the Environment in the body" });

        await _environment2DRepository.UpdateAsync(environment);

        return Ok(environment);
    }
}
