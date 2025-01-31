using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = 9.81f;
    private Vector2 moveInput;
    private CharacterController controller;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Update()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection); // Convierte a coordenadas locales

        // Aplicar gravedad manualmente
        if (!controller.isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime; // Aplicar gravedad solo si no está en el suelo
        }
        else
        {
            velocity.y = -2f; // Mantener contacto con el suelo
        }

        // Movimiento horizontal + caída por gravedad
        controller.Move((moveDirection * speed + velocity) * Time.deltaTime);
    }
}