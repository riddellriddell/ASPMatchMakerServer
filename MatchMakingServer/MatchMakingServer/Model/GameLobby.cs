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

        public DateTime TimeOfLastHostActivity { get; set; }
    }
}
