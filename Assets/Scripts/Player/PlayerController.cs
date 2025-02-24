using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;

    private Vector2 moveInput;
    private Vector3 velocity;

    private CharacterController controller;

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
        moveDirection = transform.TransformDirection(moveDirection);
        
        if (!controller.isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        controller.Move((moveDirection * speed + velocity) * Time.deltaTime);
    }

    public void OnJump(InputValue value)
    {
        Debug.Log("Espacio detectado");
        if (controller.isGrounded)
        {
            Debug.Log("Jump");
            velocity.y = Mathf.Sqrt(1.2f * gravity * jumpHeight);
        }
    }
}