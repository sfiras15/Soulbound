using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0.05f;


    private CinemachineVirtualCamera virtualCamera;

    private bool isRotating = false;
    private Vector3 lastMousePosition;

    [SerializeField] private float minZoomDistance = 2.0f;
    [SerializeField] private float maxZoomDistance = 10.0f;
    [SerializeField] private float zoomSpeed = 10.0f;
    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            RotateCamera();
        }

        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(zoomInput);
    }

    private void RotateCamera()
    {
        float mouseX = Input.mousePosition.x - lastMousePosition.x;
        lastMousePosition = Input.mousePosition;

        float rotationAmount = mouseX * rotationSpeed;

        // Apply rotation amount to the Y-axis rotation
        Vector3 currentRotation = virtualCamera.transform.rotation.eulerAngles;
        float newYRotation = currentRotation.y + rotationAmount;

        virtualCamera.transform.rotation = Quaternion.Euler(currentRotation.x, newYRotation, currentRotation.z);
    }
    private void ZoomCamera(float zoomInput)
    {
        // Calculate new zoom distance based on input
        float newZoomDistance = virtualCamera.m_Lens.OrthographicSize - (zoomInput * zoomSpeed);
        newZoomDistance = Mathf.Clamp(newZoomDistance, minZoomDistance, maxZoomDistance);

        // Update the camera's orthographic size for zooming
        virtualCamera.m_Lens.OrthographicSize = newZoomDistance;
    }
}
