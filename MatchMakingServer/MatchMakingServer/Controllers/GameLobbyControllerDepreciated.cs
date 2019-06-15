//using MatchMakingServer.Model;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MatchMakingServer.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class GameLobbyControllerDepreciated : ControllerBase
//    {
//        public class GetLobbyReply
//        {
//            public bool IsExistingLobby { get; set; }
//            public int Id { get; set; }
//        }

//        public class PostConnectionRequest
//        {
//            public int Id { get; set; }
//            public string ConnectionRequest { get; set; }
//        }

//        public class PostConnectionRequestReponse
//        {
//            public int Id { get; set; }
//        }

//        public class PostConnectionReply
//        {
//            public int Id { get; set; }
//            public string ConnectionaReply { get; set; }
//        }

//        public class GetConnectionRequestsReply
//        {
//            public int ConnectionRequestId { get; set; }
//            public string ConnectionaReply { get; set; }
//        }

//        public enum RequestsTypes
//        {
//            Connection = 0,
//            Lobby = 1
//        }


//        private readonly GameLobbyContext _context;

//        public GameLobbyControllerDepreciated(GameLobbyContext context)
//        {
//            _context = context;
//        }

//        // GET: api/GameLobby
//        [HttpGet]
//        public async Task<IActionResult> GetGameLobbyData()
//        {
//            //make sure the database is set up
//            await _context.Database.EnsureCreatedAsync();

//            ////try and find a game lobby
//            //GameLobby gameLobby = await FindGameLobby(_context);
//            //
//            ////check if game lobby was found 
//            //if (gameLobby != null)
//            //{
//            //    return Ok(new GetLobbyReply() { IsExistingLobby = true, Id = gameLobby.Id });
//            //}

//            //create game lobby
//            GameLobby gameLobby = await CreateGameLobby(_context);

           

//            //check if game lobby was found 
//            if (gameLobby != null)
//            {
//                return Ok(new GetLobbyReply() { IsExistingLobby = false, Id = gameLobby.Id });
//            }

//            //try and find a lobby near by 
//            return BadRequest();
//        }

//        // GET: api/GameLobby/5
//        [HttpGet("{type}")]
//        public async Task<IActionResult> GetGameLobby([FromRoute] RequestsTypes type, int id)
//        {
//            if (type == RequestsTypes.Lobby)
//            {
//                //make sure the database is set up
//                await _context.Database.EnsureCreatedAsync();

//                if (!ModelState.IsValid)
//                {
//                    return BadRequest(ModelState);
//                }

//                var gameLobby = await _context.GameLobbyData.FindAsync(id);

//                if (gameLobby == null)
//                {
//                    return NotFound();
//                }

//                return Ok(gameLobby.ConnectionRequests.Count);
//            }

//            if (type == RequestsTypes.Connection)
//            {
//                //make sure the database is set up
//                await _context.Database.EnsureCreatedAsync();

//                if (!ModelState.IsValid)
//                {
//                    return BadRequest(ModelState);
//                }

//                var connectionRequest = await _context.ConnectionRequestData.FindAsync(id);

//                if (connectionRequest == null)
//                {
//                    return NotFound();
//                }

//                return Ok(connectionRequest);
//            }

//            return BadRequest();
//        }

//        // PUT: api/GameLobby/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutGameLobby([FromRoute] int id, [FromBody] PostConnectionReply connectionReply)
//        {
//            //make sure the database is set up
//            await _context.Database.EnsureCreatedAsync();

//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            if (id != connectionReply.Id)
//            {
//                return BadRequest();
//            }

//            ConnectionRequest connectionRequest = await _context.ConnectionRequestData.FindAsync(id);

//            if (connectionRequest == null)
//            {
//                return NotFound();
//            }

//            connectionRequest.Reply = connectionReply.ConnectionaReply;

//            _context.Entry(connectionRequest).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!GameLobbyExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // POST: api/GameLobby
//        [HttpPost]
//        public async Task<IActionResult> PostGameLobby([FromBody] PostConnectionRequest connectionRequest)
//        {
//            //make sure the database is set up
//            await _context.Database.EnsureCreatedAsync();

//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            //try and find game lobby
//            GameLobby gameLobby = await _context.GameLobbyData.FindAsync(connectionRequest.Id);

//            if (gameLobby == null)
//            {
//                return NotFound();
//            }

//            //create connection request 
//            ConnectionRequest newConnectionRequest = new ConnectionRequest
//            {
//                TimeOfRequests = DateTime.UtcNow,
//                Request = connectionRequest.ConnectionRequest,
//                TimeOfReply = DateTime.UtcNow,
//                Reply = ""
//            };

             

//            _context.ConnectionRequestData.Add(newConnectionRequest);
//            await _context.SaveChangesAsync();

//            //link connection to lobby 
//            gameLobby.ConnectionRequests.Add(newConnectionRequest);

//            _context.Entry(gameLobby).State = EntityState.Modified;
//            await _context.SaveChangesAsync();

//            return Ok(newConnectionRequest);
//            // return Ok(new PostConnectionRequestReponse() { Id = newConnectionRequest.Id });
//            //return CreatedAtAction("PostConnectionRequest", new { ID = newConnectionRequest.Id }, newConnectionRequest);
//        }

//        // DELETE: api/GameLobby/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteGameLobby([FromRoute] int id)
//        {
//            //make sure the database is set up
//            await _context.Database.EnsureCreatedAsync();

//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var gameLobby = await _context.GameLobbyData.FindAsync(id);
//            if (gameLobby == null)
//            {
//                return NotFound();
//            }

//            //remove all connection requests 
//            foreach (ConnectionRequest request in gameLobby.ConnectionRequests)
//            {
//                _context.ConnectionRequestData.Remove(request);
//            }

//            _context.GameLobbyData.Remove(gameLobby);
//            await _context.SaveChangesAsync();

//            return Ok(gameLobby);
//        }

//        private bool GameLobbyExists(int id)
//        {
//            return _context.GameLobbyData.Any(e => e.Id == id);
//        }

//        private async Task<GameLobby> FindGameLobby(GameLobbyContext context)
//        {
//            //check if any exist 
//            if (context.GameLobbyData.Count() == 0)
//            {
//                return null;
//            }

//            //find a game loby
//            return await context.GameLobbyData.FirstOrDefaultAsync();
//        }

//        private async Task<GameLobby> CreateGameLobby(GameLobbyContext context)
//        {
//            //create new lobby
//            GameLobby newGameLobby = new GameLobby()
//            {
//                TimeOfLastHostActivity = DateTime.UtcNow
//            };

//            await _context.ConnectionRequestData.ForEachAsync(x => AddConnection(newGameLobby, x));

//            context.GameLobbyData.Add(newGameLobby);
            
//            await _context.SaveChangesAsync();

//            return newGameLobby;
//        }

//        private void AddConnection(GameLobby lobby,ConnectionRequest request)
//        {
//            lobby.ConnectionRequests.Add(request);
//        }
//    }
//}