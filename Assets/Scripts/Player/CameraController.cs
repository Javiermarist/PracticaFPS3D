using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sensitivity;
    public Transform playerBody;
    private float rotationX;
    private float rotationY;
    private Vector2 entradaRaton;
    
    public void OnLook(InputValue value)
    {
        entradaRaton = value.Get<Vector2>();
    }
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void ChangeGun(Transform gun)
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerBody = gun;
    }

    void Update()
    {
        float mouseX = entradaRaton.x * sensitivity * Time.deltaTime;
        float mouseY = entradaRaton.y * sensitivity * Time.deltaTime;
        rotationY -= mouseY;
        rotationY = Mathf.Clamp(rotationY, -80f, 80f);
        rotationX += mouseX;
        playerBody.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);
    }
}
