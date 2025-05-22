using Microsoft.AspNetCore.Mvc;
using NasaMeteoriteSomeServices.Services.Interfaces;


namespace NasaMeteoriteService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeteoritesController : ControllerBase
    {
        private readonly IMeteoriteService _meteoriteService;

        public MeteoritesController(IMeteoriteService meteoriteService)
        {
            _meteoriteService = meteoriteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var meteorites = await _meteoriteService.GetAllAsync();
            return Ok(meteorites);
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFilteredGroupedMeteorites(
            [FromQuery] int? yearFrom,
            [FromQuery] int? yearTo,
            [FromQuery] string? recclass,
            [FromQuery] string? nameContains,
            [FromQuery] string sortBy = "year")
        {
            var result = await _meteoriteService.GetFilteredGroupedMeteoritesAsync(
                yearFrom, yearTo, recclass, nameContains, sortBy);

            return Ok(result);
        }
    }
}
