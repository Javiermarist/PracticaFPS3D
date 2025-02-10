using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colision detectada con " + collision.gameObject.name);
        Destroy(gameObject);
    }
}