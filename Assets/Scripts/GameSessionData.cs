public class GameSessionData
{
    public GameState State { get; set; } = GameState.Playing;
    public int Score { get; set; }
    public int TilesRemainingInSpawn { get; set; }
}
