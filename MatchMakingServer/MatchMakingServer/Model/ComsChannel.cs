using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMakingServer.Model
{
    public class ComsChannel
    {
        public int Id { get; set; }

        public int GameLobbyId { get; set; }

        public int OwnerID { get; set; }

        public string OwnerOutData { get; set; }

        public DateTime TimeOfLastOutAction { get; set; }

        public string OwnerInData { get; set; }       

        public DateTime TimeOfLastInAction { get; set; }
    }
}