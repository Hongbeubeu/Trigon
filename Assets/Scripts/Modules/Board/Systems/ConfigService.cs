public class ConfigService
{
    public AppConfig App { get; }
    public LogicConfig Logic { get; }
    public GameViewConfig GameView { get; }

    public ConfigService(AppConfig app, LogicConfig logic, GameViewConfig gameView)
    {
        App = app;
        Logic = logic;
        GameView = gameView;
    }
}
