public class DataService
{
    public BoardData Board { get; } = new();
    public GameSessionData Session { get; } = new();

    public void ResetSession()
    {
        Session.State = GameState.Playing;
        Session.Score = 0;
        Session.TilesRemainingInSpawn = 0;
        Board.ResetOccupancy();
    }
}
