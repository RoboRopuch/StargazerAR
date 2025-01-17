using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private float RotationSpeed = 5f;

    [SerializeField]
    private bool SmoothRotation = false;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera != null)
        {
            if (SmoothRotation)
            {
                // Smooth rotation
                Quaternion targetRotation = Quaternion.LookRotation(
                    transform.position - mainCamera.transform.position,
                    mainCamera.transform.up
                );
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
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

    public void ToggleSmoothRotation(bool enable)
    {
        SmoothRotation = enable;
    }

    public void SetRotationSpeed(float speed)
    {
        RotationSpeed = speed;
    }
}
