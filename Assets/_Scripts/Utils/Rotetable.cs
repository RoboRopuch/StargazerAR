using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rotatable : MonoBehaviour
{
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private bool isInverted = false;

    private InputAction pressed;
    private InputAction axis;
    private InputAction screenPos;

    private Transform cam;
    private bool rotateAllowed;
    private Vector2 rotation;
    private Vector2 curScreenPos;

    private bool shouldCheckRaycast;

    private void Awake()
    {
        cam = Camera.main.transform;
        SetupInputActions();

        pressed.Enable();
        axis.Enable();
        screenPos.Enable();

        screenPos.performed += context => { curScreenPos = context.ReadValue<Vector2>(); };
        pressed.performed += _ => { shouldCheckRaycast = true; };
        pressed.canceled += _ => { rotateAllowed = false; shouldCheckRaycast = false; };
        axis.performed += context => { rotation = context.ReadValue<Vector2>(); };
    }

    private void SetupInputActions()
    {
        pressed = new InputAction("Pressed", InputActionType.Button);
        pressed.AddBinding("<Mouse>/leftButton");
        pressed.AddBinding("<Touchscreen>/press");

        axis = new InputAction("Axis", InputActionType.Value);
        axis.AddBinding("<Mouse>/delta");
        axis.AddBinding("<Touchscreen>/delta");

        screenPos = new InputAction("ScreenPos", InputActionType.Value);
        screenPos.AddBinding("<Pointer>/position");
    }

    private void FixedUpdate()
    {
        if (shouldCheckRaycast)
        {
            Ray ray = Camera.main.ScreenPointToRay(curScreenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    Debug.Log(transform);
                    StartCoroutine(Rotate());
                }

                shouldCheckRaycast = false;
            }
        }
    }

    private IEnumerator Rotate()
    {
        rotateAllowed = true;

        while (rotateAllowed)
        {
            Vector2 adjustedRotation = rotation * speed;
            transform.Rotate(Vector3.up * (isInverted ? 1 : -1), adjustedRotation.x, Space.World);
            transform.Rotate(cam.right * (isInverted ? -1 : 1), adjustedRotation.y, Space.World);

            yield return null;
        }
    }

    private void OnDestroy()
    {
        screenPos.performed -= context => { curScreenPos = context.ReadValue<Vector2>(); };
        pressed.performed -= _ => { shouldCheckRaycast = true; };
        pressed.canceled -= _ => { rotateAllowed = false; shouldCheckRaycast = false; };
        axis.performed -= context => { rotation = context.ReadValue<Vector2>(); };

        pressed.Disable();
        axis.Disable();
        screenPos.Disable();
    }
}
