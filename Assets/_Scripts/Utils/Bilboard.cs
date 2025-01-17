using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private float rotationSpeed = 5f; // Rotation speed for smoothing

    [SerializeField]
    private bool smoothRotation = false; // Toggle for smooth rotation, default is true

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera != null)
        {
            if (smoothRotation)
            {
                // Smooth rotation
                Quaternion targetRotation = Quaternion.LookRotation(
                    transform.position - mainCamera.transform.position,
                    mainCamera.transform.up
                );
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
            else
            {
                // Instant snapping to face the camera
                transform.LookAt(
                    transform.position + mainCamera.transform.rotation * Vector3.forward,
                    mainCamera.transform.rotation * Vector3.up
                );
            }
        }
    }

    // Method to toggle smooth rotation at runtime
    public void ToggleSmoothRotation(bool enable)
    {
        smoothRotation = enable;
    }

    // Optional: Set rotation speed dynamically
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }
}
