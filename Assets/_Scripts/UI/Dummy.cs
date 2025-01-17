using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dummy : MonoBehaviour
{
    [SerializeField] private float Speed = 0.1f;
    [SerializeField] private bool IsInverted = false;

    private InputAction Pressed;
    private InputAction Axis;
    private InputAction ScreenPos;

    private Transform Cam;
    private bool RotateAllowed;
    private Vector2 Rotation;
    private Vector2 CurScreenPos;

    private bool ShouldCheckRaycast;

    private void Awake()
    {
        Cam = Camera.main.transform;
        SetupInputActions();

        Pressed.Enable();
        Axis.Enable();
        ScreenPos.Enable();

        ScreenPos.performed += context => { CurScreenPos = context.ReadValue<Vector2>(); };
        Pressed.performed += _ => { ShouldCheckRaycast = true; };
        Pressed.canceled += _ => { RotateAllowed = false; ShouldCheckRaycast = false; };
        Axis.performed += context => { Rotation = context.ReadValue<Vector2>(); };
    }

    private void SetupInputActions()
    {
        Pressed = new InputAction("Pressed", InputActionType.Button);
        Pressed.AddBinding("<Mouse>/leftButton");
        Pressed.AddBinding("<Touchscreen>/press");

        Axis = new InputAction("Axis", InputActionType.Value);
        Axis.AddBinding("<Mouse>/delta");
        Axis.AddBinding("<Touchscreen>/delta");

        ScreenPos = new InputAction("ScreenPos", InputActionType.Value);
        ScreenPos.AddBinding("<Pointer>/position");
    }

    private void FixedUpdate()
    {
        if (ShouldCheckRaycast)
        {
            Ray ray = Camera.main.ScreenPointToRay(CurScreenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    StartCoroutine(Rotate());
                }

                ShouldCheckRaycast = false;
            }
        }
    }

    private IEnumerator Rotate()
    {
        RotateAllowed = true;

        while (RotateAllowed)
        {
            Vector2 adjustedRotation = Rotation * Speed;
            transform.Rotate(Vector3.up * (IsInverted ? 1 : -1), adjustedRotation.x, Space.World);
            transform.Rotate(Cam.right * (IsInverted ? -1 : 1), adjustedRotation.y, Space.World);

            yield return null;
        }
    }

    private void OnDestroy()
    {
        ScreenPos.performed -= context => { CurScreenPos = context.ReadValue<Vector2>(); };
        Pressed.performed -= _ => { ShouldCheckRaycast = true; };
        Pressed.canceled -= _ => { RotateAllowed = false; ShouldCheckRaycast = false; };
        Axis.performed -= context => { Rotation = context.ReadValue<Vector2>(); };

        Pressed.Disable();
        Axis.Disable();
        ScreenPos.Disable();
    }
}
