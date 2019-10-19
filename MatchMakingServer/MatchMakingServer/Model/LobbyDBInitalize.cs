namespace MatchMakingServer.Model
{
    public static class LobbyDBInitalize
    {

        public static void Initialize(GameLobbyContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

        }
    }
}
