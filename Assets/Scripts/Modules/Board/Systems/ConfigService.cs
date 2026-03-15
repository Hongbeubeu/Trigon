public class ConfigService
{
    public LogicConfig Logic { get; }
    public GameViewConfig GameView { get; }

    public ConfigService(LogicConfig logic, GameViewConfig gameView)
    {
        Logic = logic;
        GameView = gameView;
    }
}
