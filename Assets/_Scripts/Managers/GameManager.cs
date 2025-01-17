using System.Collections;
using UnityEngine;


public class GameStateManager : StaticInstance<GameStateManager>
{
    private GameState CurrentState;

    public InitializeFirebaseState InitializeFirebaseState = new();
    public CheckPermissionsState CheckPermissionsState = new();
    public LackingPermissionState LackingPermissionState = new();
    public StartServices StartLocalizationServiceState = new();
    public FetchLocalizationndCompassData FetchSituatedDataState = new();
    public ErrorInitializingLocation ErrorInitializingLocation = new();
    public FetchDataState FetchDataState = new();
    public ErrorFechingDataState ErrorFechingDataState = new();
    public SpawnObjectsState SpawnObjectsState = new();

    public void SwitchState(GameState newState)
    {
        if (CurrentState == newState)
        {
            Debug.LogWarning($"Already in state: {newState}");

        }

        CurrentState = newState;
        CurrentState.EnterState(this);
    }


    void Start()
    {
        CurrentState = InitializeFirebaseState;
        CurrentState.EnterState(this);
    }

    void Update()
    {
        CurrentState.UpdateState(this);
    }

}


public abstract class GameState
{
    public abstract void EnterState(GameStateManager manager);
    public abstract void UpdateState(GameStateManager manager);

}


public class InitializeFirebaseState : GameState
{
    public override void EnterState(GameStateManager manager)
    {
        FirebaseManager.Instance.Initialize();
        manager.SwitchState(manager.CheckPermissionsState);
    }

    public override void UpdateState(GameStateManager manager)
    {

    }

}

public class CheckPermissionsState : GameState
{
    public override void EnterState(GameStateManager manager)
    {
        ContextManager.Instance.CheckLocationPermission();

        if (ContextManager.Instance.IsLocationEnabledByUser)
        {
            manager.SwitchState(manager.StartLocalizationServiceState);
        }
        else
        {
            manager.SwitchState(manager.LackingPermissionState);
        }

    }

    public override void UpdateState(GameStateManager manager)
    {

    }
}

public class LackingPermissionState : GameState
{
    public override void EnterState(GameStateManager manager)
    {
        manager.SwitchState(manager.FetchDataState);
    }

    public override void UpdateState(GameStateManager manager)
    {

    }

}

public class ErrorInitializingLocation : GameState
{
    public override void EnterState(GameStateManager manager)
    {
        Debug.Log("Was not able to start location service");
    }

    public override void UpdateState(GameStateManager manager)
    {

    }

}

public class ErrorFechingDataState : GameState
{
    public override void EnterState(GameStateManager manager)
    {
        Debug.Log("Was not able to fetc data from database or local storage");
    }

    public override void UpdateState(GameStateManager manager)
    {

    }

}


public class StartServices : GameState
{

    void InitialiyeServies(GameStateManager manager)
    {
        ContextManager.Instance.ActivateSensors(
            () =>
            {
                manager.SwitchState(manager.FetchSituatedDataState);
            },
            error =>
            {
                manager.SwitchState(manager.ErrorInitializingLocation);
            });

    }

    public override void EnterState(GameStateManager manager)
    {
        InitialiyeServies(manager);
    }

    public override void UpdateState(GameStateManager manager)
    {

    }

}

public class FetchLocalizationndCompassData : GameState
{
    public override void EnterState(GameStateManager manager)
    {
        ContextManager.Instance.UpdateLocationReading();
        manager.SwitchState(manager.FetchDataState);
    }

    public override void UpdateState(GameStateManager manager)
    {

    }

}

public class FetchDataState : GameState
{
    public override void EnterState(GameStateManager manager)
    {
        manager.StartCoroutine(WaitForDataAndTransition(manager));
    }

    private IEnumerator WaitForDataAndTransition(GameStateManager manager)
    {
        if (!FirebaseManager.Instance.IsDataLoaded)
        {
            yield return FirebaseManager.Instance.WaitForDataLoad();
        }

        manager.SwitchState(manager.SpawnObjectsState);
    }

    public override void UpdateState(GameStateManager manager)
    {
    }
}


public class SpawnObjectsState : GameState
{
    public override void EnterState(GameStateManager manager)
    {
        UnitManager.Instance.SpawnUnits();

    }

    public override void UpdateState(GameStateManager manager)
    {
    }

}
