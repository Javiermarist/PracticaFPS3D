using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}