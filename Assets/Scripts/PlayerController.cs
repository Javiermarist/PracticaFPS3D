using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed;
    
    // player movement with input system onMove
    public void OnMove(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>();
        Vector3 move = new Vector3(inputVector.x, 0, inputVector.y);
        transform.position += move * speed * Time.deltaTime;
    }
}
