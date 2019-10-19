using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMakingServer.Model
{
    //this represents a game connection node
    public class GameLobby
    {
        public int Id { get; set; }

        public int OwnerId { get; set; }
        
        public int PlayersInLobby { get; set; }

        public int State { get; set; }

        public long TimeOfLastActivity { get; set; }
    }
}
