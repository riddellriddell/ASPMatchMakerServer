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

    public class CreateComsChannelSignal
    {
        public int GameLobbyId { get; set; }
        public int OwnerID { get; set; }
        public string InitalData { get; set; }
    }

    public class UpdateChannel
    {
        public bool Owner { get; set; }
        public bool IsWriteCommand { get; set; }
        public string Data { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ComsController : ControllerBase
    {
        private const string SignalSepparationCharacter = "|";

        private readonly GameLobbyContext _context;

        public ComsController(GameLobbyContext context)
        {
            _context = context;
        }

        // GET: api/ComsController
        [HttpGet]
        public IEnumerable<ComsChannel> GetComsController()
        {
            return _context.ComsData;
        }

        // GET: api/ComsController/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComsController([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var connectionRequest = await _context.ComsData.FindAsync(id);

            if (connectionRequest == null)
            {
                return NotFound();
            }

            return Ok(connectionRequest);
        }

        // PUT: api/ComsController/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComsController([FromRoute] int id, [FromBody] UpdateChannel updateData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ComsChannel connection = await _context.ComsData.FindAsync(id);

            if(connection == null)
            {
                return NotFound();
            }

            string signalData = string.Empty;

            if(updateData.IsWriteCommand)
            {
                AddSignal(connection, updateData);
            }
            else
            {
                signalData = ReadSignal(connection, updateData);
            }
          

            _context.Entry(connection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConnectionRequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (updateData.IsWriteCommand)
            {
                return NoContent();
            }
            else
            {
                return Ok(signalData);
            }
        }

        // POST: api/ConnectionRequest
        [HttpPost]
        public async Task<IActionResult> PostComsController([FromBody] CreateComsChannelSignal connectionDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ComsChannel connectionRequest = CreateConnection(connectionDetails);
           
            _context.ComsData.Add(connectionRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConnectionRequest", new { id = connectionRequest.Id }, connectionRequest);
        }

        // DELETE: api/ComsController/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComsController([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var connectionRequest = await _context.ComsData.FindAsync(id);
            if (connectionRequest == null)
            {
                return NotFound();
            }

            _context.ComsData.Remove(connectionRequest);
            await _context.SaveChangesAsync();

            return Ok(connectionRequest);
        }

        private bool ConnectionRequestExists(int id)
        {
            return _context.ComsData.Any(e => e.Id == id);
        }

        private ComsChannel CreateConnection(CreateComsChannelSignal newConnection)
        {
            ComsChannel connectionRequestEntry = new ComsChannel();
            connectionRequestEntry.GameLobbyId = newConnection.GameLobbyId;
            connectionRequestEntry.OwnerID = newConnection.OwnerID;
            connectionRequestEntry.OwnerOutData = newConnection.InitalData;
            connectionRequestEntry.TimeOfLastOutAction = DateTime.UtcNow;

            return connectionRequestEntry;
        }

        private void AddSignal(ComsChannel channel, UpdateChannel signal)
        {
            AddSignal(channel, signal.Owner, signal.Data);
        }

        private void AddSignal(ComsChannel channel , bool isOwner, string data)
        {
            if(isOwner)
            {
                channel.OwnerOutData = channel.OwnerOutData + SignalSepparationCharacter + data;
            }
            else
            {
                channel.OwnerInData = channel.OwnerInData + SignalSepparationCharacter + data;
            }
        }

        private string ReadSignal(ComsChannel channel,UpdateChannel signal)
        {
            string output = string.Empty;

            if(signal.Owner == true)
            {
                output = channel.OwnerInData;
                channel.OwnerInData = string.Empty;
            }
            else
            {
                output = channel.OwnerOutData;
                channel.OwnerOutData = string.Empty;
            }
           
            return output;
        }
    }
}