using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform; // Asigna la cámara en el Inspector
    public float sensitivity = 2f; // Sensibilidad del mouse
    public float maxYAngle = 80f; // Límite de rotación vertical

    private Vector2 lookInput;
    private float verticalRotation = 0f;

    // bloquea el cursor para que no salga de la pantalla
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>(); // Leer el movimiento del ratón
    }

    void Update()
    {
        // Obtener movimiento del mouse
        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        // Rotar el jugador en el eje Y (horizontal)
        transform.Rotate(Vector3.up * mouseX);

        // Rotar la cámara en el eje X (vertical), limitando el ángulo
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxYAngle, maxYAngle);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}