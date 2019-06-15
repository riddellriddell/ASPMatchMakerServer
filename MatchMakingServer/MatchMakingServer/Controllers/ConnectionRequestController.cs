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

    public class PostConnection
    {
        public int GameLobbyId { get; set; }
        public string Data { get; set; }
    }

    public class PostConnectionReply
    {
        public string Reply { get; set; }
    }
    

    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionRequestController : ControllerBase
    {
        private readonly GameLobbyContext _context;

        public ConnectionRequestController(GameLobbyContext context)
        {
            _context = context;
        }

        // GET: api/ConnectionRequest
        [HttpGet]
        public IEnumerable<ConnectionRequest> GetConnectionRequestData()
        {
            return _context.ConnectionRequestData;
        }

        // GET: api/ConnectionRequest/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConnectionRequest([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var connectionRequest = await _context.ConnectionRequestData.FindAsync(id);

            if (connectionRequest == null)
            {
                return NotFound();
            }

            return Ok(connectionRequest);
        }

        // PUT: api/ConnectionRequest/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConnectionRequest([FromRoute] int id, [FromBody] PostConnectionReply connectionReply)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ConnectionRequest connection = await _context.ConnectionRequestData.FindAsync(id);

            if(connection == null)
            {
                return NotFound();
            }

            connection = AddReply(connection, connectionReply.Reply);

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

            return NoContent();
        }

        // POST: api/ConnectionRequest
        [HttpPost]
        public async Task<IActionResult> PostConnectionRequest([FromBody] PostConnection connectionDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ConnectionRequest connectionRequest = CreateConnection(connectionDetails);
           
            _context.ConnectionRequestData.Add(connectionRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConnectionRequest", new { id = connectionRequest.Id }, connectionRequest);
        }

        // DELETE: api/ConnectionRequest/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConnectionRequest([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var connectionRequest = await _context.ConnectionRequestData.FindAsync(id);
            if (connectionRequest == null)
            {
                return NotFound();
            }

            _context.ConnectionRequestData.Remove(connectionRequest);
            await _context.SaveChangesAsync();

            return Ok(connectionRequest);
        }

        private bool ConnectionRequestExists(int id)
        {
            return _context.ConnectionRequestData.Any(e => e.Id == id);
        }

        private ConnectionRequest CreateConnection(PostConnection newConnection)
        {
            ConnectionRequest connectionRequestEntry = new ConnectionRequest();
            connectionRequestEntry.GameLobbyId = newConnection.GameLobbyId;
            connectionRequestEntry.Request = newConnection.Data;
            connectionRequestEntry.TimeOfRequests = DateTime.UtcNow;

            return connectionRequestEntry;
        }

        private ConnectionRequest AddReply(ConnectionRequest connection, string connectionReply)
        {
            connection.Reply = connectionReply;
            connection.TimeOfReply = DateTime.UtcNow;

            return connection;
        }
    }
}