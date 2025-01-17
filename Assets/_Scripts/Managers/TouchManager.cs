using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : Singleton<TouchManager>
{
    public static event Action<RaycastHit> OnObjectHit;
    public static event Action OnVoidHit;

    private InputAction Pressed;
    private InputAction ScreenPosition;

    private Vector2 LastUserInteracionPosition;
    [SerializeField] private LayerMask InteractionLayer;


    private void DefineUserInteractions()
    {
        Pressed = new InputAction("Pressed", InputActionType.Button);
        ScreenPosition = new InputAction("ScreenPos", InputActionType.Value);
    }

    private void ConnectUserActionsToDevices()
    {
        Pressed.AddBinding("<Mouse>/leftButton");
        Pressed.AddBinding("<Touchscreen>/press");
        ScreenPosition.AddBinding("<Pointer>/position");
    }

    private void OnPressed(InputAction.CallbackContext context) => PerformRaycast();
    private void OnScreenPosChanged(InputAction.CallbackContext context) => LastUserInteracionPosition = context.ReadValue<Vector2>();

    private void AssignUserInteractionResponses()
    {
        Pressed.performed += OnPressed;
        ScreenPosition.performed += OnScreenPosChanged;
    }

    private void StartMonitoringUserInterations()
    {
        Pressed.Enable();
        ScreenPosition.Enable();
    }

    private void StopMonitoringUserInterations()
    {
        Pressed.Enable();
        ScreenPosition.Enable();
    }

    private void CancelUserInteractionResponses()
    {
        Pressed.performed += OnPressed;
        ScreenPosition.performed += OnScreenPosChanged;
    }

    protected override void Awake()
    {

        DefineUserInteractions();
        ConnectUserActionsToDevices();
        AssignUserInteractionResponses();
        StartMonitoringUserInterations();

        base.Awake();
    }


    private void PerformRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(LastUserInteracionPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, InteractionLayer))
        {
            OnObjectHit?.Invoke(hit);
        }
        else
        {
            OnVoidHit?.Invoke();
        }
    }


    private void OnDestroy()
    {
        StopMonitoringUserInterations();
        CancelUserInteractionResponses();
    }
}
