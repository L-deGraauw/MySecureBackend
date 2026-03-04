using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;

namespace MySecureBackend.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class Object2DController : ControllerBase
    {
        private readonly IObject2DRepository _object2DRepository;

        public Object2DController(IObject2DRepository object2DRepository)
        {
            _object2DRepository = object2DRepository;
        }

        [HttpGet(Name = "GetObject2Ds")]
        public async Task<ActionResult<List<Object2D>>> GetAsync()
        {
            var objects = await _object2DRepository.SelectAsync();
            return Ok(objects);
        }

        [HttpGet("environment/{environmentId}", Name = "GetObject2DsByEnvironment")]
        public async Task<ActionResult<List<Object2D>>> GetByEnvironmentAsync(Guid environmentId)
        {
            var objects = await _object2DRepository.SelectByEnvironmentAsync(environmentId);
            return Ok(objects);
        }

        [HttpGet("{object2DId}", Name = "GetObject2DById")]
        public async Task<ActionResult<Object2D>> GetByIdAsync(Guid object2DId)
        {
            var object2D = await _object2DRepository.SelectAsync(object2DId);

            if (object2D == null)
                return NotFound(new ProblemDetails { Detail = $"Object2D {object2DId} not found" });

            return Ok(object2D);
        }

        [HttpPost(Name = "AddObject2D")]
        public async Task<ActionResult<Object2D>> AddAsync(Object2D object2D)
        {
            object2D.Id = Guid.NewGuid();

            await _object2DRepository.InsertAsync(object2D);

            return CreatedAtRoute("GetObject2DById", new { object2DId = object2D.Id }, object2D);
        }

        [HttpPut("{object2DId}", Name = "UpdateObject2D")]
        public async Task<ActionResult<Object2D>> UpdateAsync(Guid object2DId, Object2D object2D)
        {
            var existingObject2D = await _object2DRepository.SelectAsync(object2DId);

            if (existingObject2D == null)
                return NotFound(new ProblemDetails { Detail = $"Object2D {object2DId} not found" });

            if (object2D.Id != object2DId)
                return Conflict(new ProblemDetails { Detail = "The id of the Object2D in the route does not match the id of the Object2D in the body" });

            await _object2DRepository.UpdateAsync(object2D);

            return Ok(object2D);
        }

        [HttpDelete("{object2DId}", Name = "DeleteObject2D")]
        public async Task<ActionResult> DeleteAsync(Guid object2DId)
        {
            var object2D = await _object2DRepository.SelectAsync(object2DId);

            if (object2D == null)
                return NotFound(new ProblemDetails { Detail = $"Object2D {object2DId} not found" });

            await _object2DRepository.DeleteAsync(object2DId);

            return Ok();
        }
    }
}
