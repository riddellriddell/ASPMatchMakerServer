using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMakingServer.Model
{
    public class GameLobbyContext : DbContext
    {
        public DbSet<GameLobby> GameLobbyData { get; set; }

        public DbSet<ComsChannel> ComsData { get; set; }

        public GameLobbyContext()
        {

        }

        public GameLobbyContext(DbContextOptions<GameLobbyContext> options) : base(options)
        {

        }
    }
}
