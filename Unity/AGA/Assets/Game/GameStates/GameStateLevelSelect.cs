using GameLib.Log;

public class GameStateLevelSelect : AppStateManager.AppState<GameStateLevelSelect>
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
