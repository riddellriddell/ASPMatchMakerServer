using MatchMakingServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMakingServer.CommunicationTypes
{
    public class MessageSend
    {
        public int fromId;
        public int toId;
        public string message;
    }

    public class MessageGet
    {
        public List<Message> messages;
    }
}
