using System.Collections;
using UnityEngine;


public class GameStateManager : StaticInstance<GameStateManager>
{

    private GameState currentState;
    public CheckPermissionsState checkPermissionsState = new();
    public LackingPermissionState lackingPermissionState = new();
    public StartServices startLocalizationServiceState = new();
    public FetchLocalizationndCompassData fetchSituatedDataState = new();
    public ErrorInitializingLocation errorInitializingLocation = new();
    public FetchDataState fetchDataState = new();
    public ErrorFechingDataState errorFechingDataState = new();
    public SpawnObjectsState spawnObjectsState = new();


    public bool isFirebaseReady = false;

    public void SwitchState(GameState newState)
    {
        Debug.LogWarning($"Switching to state: {newState}");
        if (currentState == newState)
        {
            Debug.LogWarning($"Already in state: {newState}");

        }


        currentState = newState;
        currentState.EnterState(this);
    }


    void Start()
    {
        FirebaseManager.Instance.Initialize();

        currentState = checkPermissionsState;
        currentState.EnterState(this);
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

}


public abstract class GameState
{
    public abstract void EnterState(GameStateManager manager);
    public abstract void UpdateState(GameStateManager manager);

}


public class CheckPermissionsState : GameState
{
    public override void EnterState(GameStateManager manager)
    {
        ContextManager.Instance.CheckLocationPermission();

        if (ContextManager.Instance.IsLocationEnabledByUser)
        {
            manager.SwitchState(manager.startLocalizationServiceState);
        }
        else
        {
            manager.SwitchState(manager.lackingPermissionState);
        };

    }

    public override void UpdateState(GameStateManager manager)
    {

    }
}

public class LackingPermissionState : GameState
{
    public override void EnterState(GameStateManager manager)
    {
        manager.SwitchState(manager.fetchDataState);
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
                manager.SwitchState(manager.fetchSituatedDataState);
            },
            error =>
            {
                manager.SwitchState(manager.errorInitializingLocation);
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
        manager.SwitchState(manager.fetchDataState);
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
        Debug.Log("Fetching data...");

        if (!FirebaseManager.Instance.IsDataLoaded)
        {
            Debug.Log("Data not yet loaded. Waiting...");
            yield return FirebaseManager.Instance.WaitForDataLoad();
        }

        Debug.Log("Data loaded. Transitioning to spawnObjectsState...");
        manager.SwitchState(manager.spawnObjectsState);
    }

    public override void UpdateState(GameStateManager manager)
    {
    }
}


public class SpawnObjectsState : GameState
{
    public override void EnterState(GameStateManager manager)
    {
        UnitManager.Instance.SpawnHeroes();

    }

    public override void UpdateState(GameStateManager manager)
    {
    }

}
