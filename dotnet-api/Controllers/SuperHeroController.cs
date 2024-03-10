using dotnet_api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnet_api.Data;

namespace dotnet_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {

        private readonly DataContext _context;

        public SuperHeroController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllHeroes()
        {
            var heroes = await _context.SuperHeroes.ToListAsync();

            return Ok(heroes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SuperHero>> GetSingleHero(int id)
        {
            var hero = await _context.SuperHeroes.FindAsync(id);

            if (hero is null)
            {
                return NotFound("Hero not found");
            }

            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult> CreateHero(SuperHero hero)
        {
            try
            {
                var result = await _context.SuperHeroes.AddAsync(hero);
                await _context.SaveChangesAsync();

                if (result.Entity == null)
                {
                    return BadRequest("Hero not created");
                }

                return Ok(result.Entity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SuperHero>> UpdateHero(int id, SuperHero hero)
        {
            System.Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(hero));
            var heroToUpdate = await _context.SuperHeroes.FindAsync(id);

            if (heroToUpdate is null)
            {
                return NotFound("Hero not found");
            }

            foreach (var property in hero.GetType().GetProperties())
            {
                if (property.Name != "Id")
                {
                    var value = property.GetValue(hero);
                    if (value != null)
                    {
                        property.SetValue(heroToUpdate, value);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(heroToUpdate);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteHero(int id)
        {
            var hero = await _context.SuperHeroes.FindAsync(id);

            if (hero is null)
            {
                return NotFound("Hero not found");
            }

            _context.SuperHeroes.Remove(hero);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Hero deleted",
                hero
            });
        }
    }
}