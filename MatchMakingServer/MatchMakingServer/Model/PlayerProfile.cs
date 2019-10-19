using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMakingServer.Model
{
    public class PlayerProfile
    {
        public int Id { get; set; }
        public long TimeOfLastActivity { get; set; }
    }
}
