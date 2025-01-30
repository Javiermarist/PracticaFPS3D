using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    
    public void OnMove(InputValue value)
    {
        Vector3 inputVector = value.Get<Vector3>();
        Vector3 move = new Vector3(inputVector.x, 0, inputVector.y);
        transform.position += move * moveSpeed * Time.deltaTime;
    }
}
