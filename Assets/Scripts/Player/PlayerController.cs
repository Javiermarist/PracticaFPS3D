using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f; // Velocidad del jugador
    [SerializeField] private float sensitivityX = 2.0f; // Sensibilidad horizontal (control del ratón para rotación del jugador)
    [SerializeField] private float sensitivityY = 2.0f; // Sensibilidad vertical (control del ratón para rotación de la cámara)
    [SerializeField] private float minimumY = -60f; // Ángulo mínimo para la rotación vertical de la cámara
    [SerializeField] private float maximumY = 60f; // Ángulo máximo para la rotación vertical de la cámara

    private float rotationX = 0f; // Rotación horizontal (Y)
    private float rotationY = 0f; // Rotación vertical (X)

    private CharacterController controller;
    private Camera camera;

    private void Start()
    {
        controller = GetComponent<CharacterController>(); // Obtener el CharacterController
        camera = Camera.main; // Obtener la cámara principal

        // Bloquear el cursor para que no se salga de la pantalla y hacerlo invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Obtener el movimiento del ratón
        rotationX += Input.GetAxis("Mouse X") * sensitivityX; // Movimiento horizontal del ratón (izquierda/derecha)
        rotationY -= Input.GetAxis("Mouse Y") * sensitivityY; // Movimiento vertical del ratón (arriba/abajo)

        // Limitar la rotación vertical de la cámara (para evitar que el jugador gire completamente)
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        // Aplicar la rotación vertical de la cámara (en el eje X)
        camera.transform.localRotation = Quaternion.Euler(rotationY, 0, 0); // Solo rotación vertical para la cámara

        // Aplicar la rotación horizontal del jugador (en el eje Y)
        transform.rotation = Quaternion.Euler(0, rotationX, 0); // Solo rotación en Y para el jugador

        // Obtener las entradas de movimiento (WASD o las flechas del teclado)
        float horizontal = Input.GetAxis("Horizontal"); // Movimiento horizontal (A/D o Izquierda/Derecha)
        float vertical = Input.GetAxis("Vertical"); // Movimiento vertical (W/S o Arriba/Abajo)

        // Calcular la dirección de movimiento en función de la rotación de la cámara
        Vector3 direction = camera.transform.forward * vertical + camera.transform.right * horizontal;

        // Asegurarse de que el movimiento solo se da en el plano horizontal (sin moverse hacia arriba/abajo)
        direction.y = 0;

        // Mover al jugador usando el CharacterController
        controller.Move(direction * speed * Time.deltaTime); // Mover el jugador con velocidad ajustable
    }
}
