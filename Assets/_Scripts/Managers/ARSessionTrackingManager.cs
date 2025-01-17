using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARSessionTracker : MonoBehaviour
{
    public GameObject SessionLostOverlay;
    private ARSessionState PreviousState = ARSessionState.None;

    private void OnEnable()
    {
        ARSession.stateChanged += OnARSessionStateChanged;
    }

    private void OnDisable()
    {
        ARSession.stateChanged -= OnARSessionStateChanged;
    }

    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {

        switch (args.state)
        {
            case ARSessionState.SessionTracking:
                if (PreviousState == ARSessionState.SessionInitializing)
                {
                    UnitManager.Instance.ForceRotationSync();
                    SessionLostOverlay.SetActive(false);
                }
                break;

            case ARSessionState.SessionInitializing:
                // Handle session reinitialization
                // Notify the UnitManager to force environment rotation sync
                if (PreviousState == ARSessionState.SessionTracking)
                {
                    SessionLostOverlay.SetActive(true);
                }
                break;

            case ARSessionState.Ready:
                break;

            case ARSessionState.Unsupported:
                break;

            case ARSessionState.None:
            case ARSessionState.NeedsInstall:
                break;

            default:
                break;
        }

        PreviousState = args.state;

    }
}
