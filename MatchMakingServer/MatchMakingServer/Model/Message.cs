using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMakingServer.Model
{
    public class Message
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int ToPlayerProfileId { get; set; }
        public int FromPlayerProfileId { get; set; }
        public string Value { get; set; }
    }
}
