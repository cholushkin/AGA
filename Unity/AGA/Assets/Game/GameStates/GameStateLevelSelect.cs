using GameLib;
using GameLib.Log;
using UnityEngine;

public class GameStateLevelSelect : AppStateManager.AppState<GameStateLevelSelect>
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
        if (Input.GetKeyDown(KeyCode.S))
        {
            print("Debug start game");
            AppStateManager.Instance.Start<GameStateGameplay>();
        }
    }
}
