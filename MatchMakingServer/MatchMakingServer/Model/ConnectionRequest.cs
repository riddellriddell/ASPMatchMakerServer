using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMakingServer.Model
{
    public class ConnectionRequest
    {
        public int Id { get; set; }

        public int GameLobbyId { get; set; }

        public string Request { get; set; }

        public DateTime TimeOfRequests { get; set; }

        public string Reply { get; set; }       

        public DateTime TimeOfReply { get; set; }
    }
}