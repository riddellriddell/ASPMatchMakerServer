using MatchMakingServer.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMakingServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameLobbyController : ControllerBase
    {
        public class GetLobbyReply
        {
            public bool IsExistingLobby { get; set; }
            public int Id { get; set; }
        }


        public class PostConnectionRequestReponse
        {
            public int Id { get; set; }
        }

        public class GetConnectionRequestsReply
        {
            public int ConnectionRequestId { get; set; }
            public string ConnectionaReply { get; set; }
        }

        private readonly GameLobbyContext _context;

        public GameLobbyController(GameLobbyContext context)
        {
            _context = context;
        }

        // GET: api/GameLobby
        [HttpGet]
        public async Task<IActionResult> GetGameLobbyData()
        {
            //make sure the database is set up
            await _context.Database.EnsureCreatedAsync();

            //try and find a game lobby
            GameLobby gameLobby = await FindGameLobby(_context);

            //check if game lobby was found 
            if (gameLobby != null)
            {
                return Ok(new GetLobbyReply() { IsExistingLobby = true, Id = gameLobby.Id });
            }

            //create game lobby
            gameLobby = await CreateGameLobby(_context);

            //check if game lobby was found 
            if (gameLobby != null)
            {
                return Ok(new GetLobbyReply() { IsExistingLobby = false, Id = gameLobby.Id });
            }

            //try and find a lobby near by 
            return BadRequest();
        }

        // GET: api/GameLobby/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameLobby([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var gameLobby = await _context.GameLobbyData.FindAsync(id);

            if (gameLobby == null)
            {
                return NotFound();
            }
                       
            var query = from request in _context.ComsData
                        where request.GameLobbyId == gameLobby.Id
                        select request;

            List<ComsChannel> connections = await query.ToListAsync();

            return Ok(connections);
        }

        // PUT: api/GameLobby/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGameLobby([FromRoute] int id, [FromBody] GameLobby gameLobby)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != gameLobby.Id)
            {
                return BadRequest();
            }

            _context.Entry(gameLobby).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameLobbyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/GameLobby
        [HttpPost]
        public async Task<IActionResult> PostGameLobby([FromBody] GameLobby gameLobby)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.GameLobbyData.Add(gameLobby);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGameLobby", new { id = gameLobby.Id }, gameLobby);
        }

        // DELETE: api/GameLobby/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGameLobby([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var gameLobby = await _context.GameLobbyData.FindAsync(id);
            if (gameLobby == null)
            {
                return NotFound();
            }

            _context.GameLobbyData.Remove(gameLobby);

            //remove all relaeted connections
            _context.ComsData.RemoveRange(_context.ComsData.Where(x =>x.GameLobbyId == id));

            await _context.SaveChangesAsync();

            return Ok(gameLobby);
        }

        private bool GameLobbyExists(int id)
        {
            return _context.GameLobbyData.Any(e => e.Id == id);
        }

        private async Task<GameLobby> FindGameLobby(GameLobbyContext context)
        {
            //check if any exist 
            if (context.GameLobbyData.Count() == 0)
            {
                return null;
            }

            //find a game loby
            return await context.GameLobbyData.FirstOrDefaultAsync();
        }

        private async Task<GameLobby> CreateGameLobby(GameLobbyContext context)
        {
            //create new lobby
            GameLobby newGameLobby = new GameLobby()
            {
                TimeOfLastHostActivity = DateTime.UtcNow
            };

            context.GameLobbyData.Add(newGameLobby);

            await _context.SaveChangesAsync();

            return newGameLobby;
        }
    }
}