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
    public class PlayerProfilesController : ControllerBase
    {
        public static async Task<PlayerProfile> GetCreatePlayerProfile(GameLobbyContext glcContext, int iPlayerID)
        {
            //check if player not is requesting a new profile 
            if (iPlayerID != int.MinValue)
            {
                //check if player exists in the database 
                PlayerProfile plpExistingProfile = await glcContext.PlayerProfileData.FirstOrDefaultAsync(plpProfile => plpProfile.Id == iPlayerID);

                if (plpExistingProfile != null)
                {
                    return plpExistingProfile;
                }
            }

            //no player profile exists create a new one 
            PlayerProfile plpNewProfile = new PlayerProfile();

            await glcContext.PlayerProfileData.AddAsync(plpNewProfile);

            try
            {
                await glcContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return plpNewProfile;
        }

        private static async Task<bool> PlayerProfileExists(GameLobbyContext glcContext, int id)
        {
            return await glcContext.PlayerProfileData.AnyAsync(e => e.Id == id);
        }


        private readonly GameLobbyContext _context;

        public PlayerProfilesController(GameLobbyContext context)
        {
            _context = context;
        }

        // GET: api/PlayerProfiles
        [HttpGet]
        public IEnumerable<PlayerProfile> GetPlayerProfileData()
        {
            return _context.PlayerProfileData;
        }

        // GET: api/PlayerProfiles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlayerProfile([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var playerProfile = await _context.PlayerProfileData.FindAsync(id);

            if (playerProfile == null)
            {
                return NotFound();
            }

            return Ok(playerProfile);
        }

        // PUT: api/PlayerProfiles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayerProfile([FromRoute] int id, [FromBody] PlayerProfile playerProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != playerProfile.Id)
            {
                return BadRequest();
            }

            _context.Entry(playerProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerProfileExists(id))
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

        // POST: api/PlayerProfiles
        [HttpPost]
        public async Task<IActionResult> PostPlayerProfile([FromBody] PlayerProfile playerProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PlayerProfileData.Add(playerProfile);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayerProfile", new { id = playerProfile.Id }, playerProfile);
        }

        // DELETE: api/PlayerProfiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayerProfile([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var playerProfile = await _context.PlayerProfileData.FindAsync(id);
            if (playerProfile == null)
            {
                return NotFound();
            }

            _context.PlayerProfileData.Remove(playerProfile);
            await _context.SaveChangesAsync();

            return Ok(playerProfile);
        }


        private bool PlayerProfileExists(int id)
        {
            return _context.PlayerProfileData.Any(e => e.Id == id);
        }


    }
}