/// <summary>
/// Volatile runtime container for tracking the active game's score
/// and the remaining tiles inside the spawning rack.
/// </summary>
public class GameSessionData
{
    public GameState State { get; set; } = GameState.Playing;
    public int Score { get; set; }
    public int TilesRemainingInSpawn { get; set; }
}
