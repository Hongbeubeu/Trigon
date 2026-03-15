public class ConfigService
{
    public LogicConfig Logic { get; }
    public GameViewConfig GameView { get; }
    public UIViewConfig UI { get; }

    public ConfigService(LogicConfig logic, GameViewConfig gameView, UIViewConfig ui)
    {
        Logic = logic;
        GameView = gameView;
        UI = ui;
    }
}
