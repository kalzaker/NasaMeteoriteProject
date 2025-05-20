using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NasaMeteoriteService.Data;
using NasaMeteoriteService.Models;

namespace NasaMeteoriteService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeteoritesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public MeteoritesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFilteredGroupedMeteorites(
            [FromQuery] int? yearFrom,
            [FromQuery] int? yearTo,
            [FromQuery] string? recclass,
            [FromQuery] string? nameContains,
            [FromQuery] string sortBy = "year" // year, count, mass
        )
        {
            var query = _dbContext.Meteorites.AsQueryable();

            if (yearFrom.HasValue)
                query = query.Where(m => m.Year.HasValue && m.Year.Value.Year >= yearFrom.Value);

            if (yearTo.HasValue)
                query = query.Where(m => m.Year.HasValue && m.Year.Value.Year <= yearTo.Value);

            if (!string.IsNullOrWhiteSpace(recclass))
                query = query.Where(m => m.Recclass == recclass);

            if (!string.IsNullOrWhiteSpace(nameContains))
                query = query.Where(m => m.Name.ToLower().Contains(nameContains.ToLower()));

            var test = await query
            .Where(m => (m.Year.Value.Year < 1000 || m.Year.Value.Year > 3000))
               .ToListAsync();

            if (test.Any())
            {
                return BadRequest(new
                {
                    Error = "Некорректные значения года",
                    ProblematicEntries = test.Select(m => new { m.Name, m.Year })
                });
            }

            var massTest = await query
    .Where(m => m.Mass > int.MaxValue)
    .ToListAsync();

            if (massTest.Any())
            {
                return BadRequest(new
                {
                    Error = "Слишком большие значения массы",
                    ProblematicEntries = massTest.Select(m => new { m.Name, m.Mass })
                });
            }

            var grouped = await query
                .Where(m => m.Year.HasValue)
                .GroupBy(m => m.Year.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Count = g.Count(),
                    TotalMass = g.Sum(m => m.Mass ?? 0)
                })
                .ToListAsync();

            grouped = sortBy.ToLower() switch
            {
                "count" => grouped.OrderByDescending(g => g.Count).ToList(),
                "mass" => grouped.OrderByDescending(g => g.TotalMass).ToList(),
                _ => grouped.OrderBy(g => g.Year).ToList()
            };

            return Ok(grouped);
        }
    }
}
