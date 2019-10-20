using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchMakingServer.Model;
using MatchMakingServer.CommunicationTypes;

namespace MatchMakingServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        public static float s_fMessageTimeOut = 5;

        public static async Task<Message> CreateMessage(GameLobbyContext glcContext, int iFrom, int iTo, string strValue)
        {
            Message msgMessage = new Message();
            msgMessage.Time = DateTime.UtcNow.Ticks;
            msgMessage.ToPlayerProfileId = iTo;
            msgMessage.FromPlayerProfileId = iFrom;

            await glcContext.AddAsync(msgMessage);

            return msgMessage;
        }

        public static bool IsMessageValid(Message msgMessage)
        {
            if(DateTime.UtcNow.Ticks - msgMessage.Time > TimeSpan.FromSeconds(s_fMessageTimeOut).Ticks)
            {
                return false;
            }

            return true;
        }

        private readonly GameLobbyContext _context;

        public MessagesController(GameLobbyContext context)
        {
            _context = context;
        }

        // GET: api/Messages
        [HttpGet]
        public IEnumerable<Message> GetMessageData()
        {
            return _context.MessageData;
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var message = await _context.MessageData.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        // PUT: api/Messages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //get message 
            List<Message> msgMessages = await _context.MessageData.Where(
                msgMessage => msgMessage.ToPlayerProfileId == id
                ).OrderBy(
                msgMessage => msgMessage.Time
                ).ToListAsync();

            //try and delete messages
            _context.MessageData.RemoveRange(msgMessages);

            //check if any messages were found
            if (msgMessages == null || msgMessages.Count == 0)
            {
                return NoContent();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }

            //return all the valid messages 
            MessageGet msgMessageValues = new MessageGet() { messages = msgMessages.Where(msgMessage => IsMessageValid(msgMessage)).ToList() };

            return Ok(msgMessageValues);
        }

        // POST: api/Messages
        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] MessageSend message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Message msgNewMessage = new Message()
            {
                FromPlayerProfileId = message.fromId,
                ToPlayerProfileId = message.toId,
                Value = message.message,
                Time = DateTime.UtcNow.Ticks
            };

            _context.MessageData.Add(msgNewMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostMessage", msgNewMessage);
        }

    }
}