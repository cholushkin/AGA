using GameLib.Log;
using UnityEngine;

public class GameStateGameplay : AppStateManager.AppState<GameStateGameplay>
{
    public override void AppStateInitialization()
    {
        LogChecker.Print("AppStateInitialization");
        gameObject.SetActive(false);
    }

    public override void AppStateEnter()
    {
        LogChecker.Print("AppStateEnter");
        gameObject.SetActive(true);
    }

    public override void AppStateLeave()
    {
        LogChecker.Print("AppStateLeave");
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            print("Debug finish game");
            AppStateManager.Instance.Start<GameStateLevelSelect>();
        }
    }
}
