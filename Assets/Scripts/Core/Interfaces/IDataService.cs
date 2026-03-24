public interface IDataService
{
    BoardData Board { get; }
    GameSessionData Session { get; }
    void ResetSession();
}
