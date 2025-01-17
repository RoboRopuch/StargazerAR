// using System;
// using UnityEngine;
// using UnityEngine.InputSystem;

// public class TouchManager : MonoBehaviour
// {
//     private InputAction pressed;
//     private InputAction screenPos;
//     private Vector2 curScreenPos;
//     public static event Action<RaycastHit> OnObjectHit;
//     public static event Action OnVoidHit;
//     [SerializeField] private LayerMask layerMask;

//     private void Awake()
//     {
//         SetupInputActions();

//         pressed.performed += OnPressed;
//         screenPos.performed += OnScreenPosChanged;

//         pressed.Enable();
//         screenPos.Enable();
//     }

//     private void SetupInputActions()
//     {
//         pressed = new InputAction("Pressed", InputActionType.Button);
//         pressed.AddBinding("<Mouse>/leftButton");
//         pressed.AddBinding("<Touchscreen>/press");

//         screenPos = new InputAction("ScreenPos", InputActionType.Value);
//         screenPos.AddBinding("<Pointer>/position");
//     }

//     private void OnPressed(InputAction.CallbackContext context)
//     {
//         PerformRaycast();
//     }

//     private void OnScreenPosChanged(InputAction.CallbackContext context)
//     {
//         curScreenPos = context.ReadValue<Vector2>();
//     }

//     private void PerformRaycast()
//     {
//         Ray ray = Camera.main.ScreenPointToRay(curScreenPos);
//         if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
//         {
//             Debug.LogWarning("raycast fired");
//             OnObjectHit?.Invoke(hit);
//         }
//         else
//         {
//             OnVoidHit?.Invoke();
//         }
//     }


//     private void OnDestroy()
//     {
//         pressed.performed -= OnPressed;
//         screenPos.performed -= OnScreenPosChanged;

//         pressed.Disable();
//         screenPos.Disable();
//     }
// }

// using System;
// using UnityEngine;
// using UnityEngine.InputSystem;

// public class TouchManager : MonoBehaviour
// {
//     public static event Action<RaycastHit> OnObjectHit;
//     public static event Action OnVoidHit;

//     private InputAction Pressed;
//     private InputAction ScreenPosition;

//     private Vector2 LastScreenPressPosition;


//     private void SetupInputActions()
//     {
//         Pressed = new InputAction("Pressed", InputActionType.Button);
//         Pressed.AddBinding("<Mouse>/leftButton");
//         Pressed.AddBinding("<Touchscreen>/press");

//         ScreenPosition = new InputAction("ScreenPos", InputActionType.Value);
//         ScreenPosition.AddBinding("<Pointer>/position");
//     }
//     private void Awake()
//     {
//         SetupInputActions();

//         Pressed.performed += OnPressed;
//         ScreenPosition.performed += OnScreenPosChanged;

//         Pressed.Enable();
//         ScreenPosition.Enable();
//     }



//     private void OnPressed(InputAction.CallbackContext context)
//     {
//         PerformRaycast();
//     }

//     private void OnScreenPosChanged(InputAction.CallbackContext context)
//     {
//         LastScreenPressPosition = context.ReadValue<Vector2>();
//     }

//     private void PerformRaycast()
//     {
//         Debug.Log("fire");
//         Ray ray = Camera.main.ScreenPointToRay(LastScreenPressPosition);

//         if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactionLayer))
//         {
//             OnObjectHit?.Invoke(hit);
//         }
//         else
//         {
//             OnVoidHit?.Invoke();
//         }
//     }


//     private void OnDestroy()
//     {
//         Pressed.performed -= OnPressed;
//         ScreenPosition.performed -= OnScreenPosChanged;

//         Pressed.Disable();
//         ScreenPosition.Disable();
//     }
// }


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
