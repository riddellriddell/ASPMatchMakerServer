using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchMakingServer.Model;

namespace MatchMakingServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameLobbieController : ControllerBase
    {
        private static async Task<GameLobby> GetCreateGameLobby(GameLobbyContext glcContext, PlayerProfile plpPlayerProfile)
        {
            //create new game lobby 
            if (glcContext.GameLobbyData.Count() == 0)
            {
                GameLobby glbNewGameLobby = CreateNewGameLobby(plpPlayerProfile);
                glbNewGameLobby.OwnerId = plpPlayerProfile.Id;
                await glcContext.GameLobbyData.AddAsync(glbNewGameLobby);

                return glbNewGameLobby;
            }

            //try and find an existing game lobby 
            return await glcContext.GameLobbyData.FirstOrDefaultAsync();
        }

        private static GameLobby CreateNewGameLobby(PlayerProfile plpPlayerProfile)
        {
            GameLobby glbNewLobby = new GameLobby();

            glbNewLobby.TimeOfLastActivity = DateTime.UtcNow;

            return glbNewLobby;
        }

        private readonly GameLobbyContext _context;

        public GameLobbieController(GameLobbyContext context)
        {
            _context = context;
        }

        // GET: api/GameLobbie
        [HttpGet]
        public IEnumerable<GameLobby> GetGameLobbyData()
        {
            return _context.GameLobbyData;
        }

        // GET: api/GameLobbie/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameLobby([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Database.EnsureCreatedAsync();

            var gameLobby = await _context.GameLobbyData.FindAsync(id);

            if (gameLobby == null)
            {
                return NotFound();
            }

            return Ok(gameLobby);
        }

        // PUT: api/GameLobbie/5
        [HttpPut]
        public async Task<IActionResult> PutGameLobby([FromBody] int id, int playerCount, int state)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Database.EnsureCreatedAsync();

            //try and get game lobby 
            GameLobby glbGameLobby = await _context.GameLobbyData.FindAsync(id);

            if (glbGameLobby == null)
            {
                return NotFound();
            }

            //set player as owner of lobby
            glbGameLobby.PlayersInLobby = playerCount;
            glbGameLobby.State = state;
            glbGameLobby.TimeOfLastActivity = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(glbGameLobby);
        }

        // POST: api/GameLobbie
        [HttpPost]
        public async Task<IActionResult> PostGameLobby([FromBody] Tuple<int, int> postRequest )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Database.EnsureCreatedAsync();

            //try and get game lobby 
            GameLobby glbGameLobby = await _context.GameLobbyData.FindAsync(postRequest.Item1);

            //get player profile
            PlayerProfile plpProfile = await PlayerProfilesController.GetCreatePlayerProfile(_context, postRequest.Item2);


            if (glbGameLobby == null)
            {
                //create new game lobby for player
                glbGameLobby = CreateNewGameLobby(plpProfile);

                //add game lobby
                await _context.AddAsync(glbGameLobby);
            }

            //set player as owner of lobby
            glbGameLobby.OwnerId = plpProfile.Id;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return CreatedAtAction("PostGameLobby", new Tuple<GameLobby, PlayerProfile>(glbGameLobby, plpProfile));
        }

        // POST: api/GameLobbie
        [HttpPost]
        [Route("matchmake")]
        public async Task<IActionResult> PostMatchMake([FromBody] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Database.EnsureCreatedAsync();

            //get player profile
            PlayerProfile plpProfile = await PlayerProfilesController.GetCreatePlayerProfile(_context, id);

            if(plpProfile == null)
            {
                return NotFound();
            }

            //try and get game lobby 
            GameLobby glbGameLobby = await GetCreateGameLobby(_context, plpProfile);

            if (glbGameLobby == null)
            {
                return NotFound();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return CreatedAtAction("PostMatchMake", new { id }, new Tuple<GameLobby, PlayerProfile>(glbGameLobby, plpProfile));
        }

        // DELETE: api/GameLobbie/5
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
            await _context.SaveChangesAsync();

            return Ok(gameLobby);
        }

        private bool GameLobbyExists(int id)
        {
            return _context.GameLobbyData.Any(e => e.Id == id);
        }
    }
}