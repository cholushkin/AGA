using GameLib.Log;

public class GameStateGameplay : AppStateManager.AppState<GameStateGameplay>
{
    public override void AppStateInitialization()
    {
        LogChecker.Print("AppStateInitialization");
    }

    public override void AppStateEnter()
    {
        LogChecker.Print("AppStateEnter");
    }

    public override void AppStateLeave()
    {
        LogChecker.Print("AppStateLeave");
    }
}
