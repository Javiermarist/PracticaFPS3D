using System;
using UnityEngine;

public class Trump : MonoBehaviour
{
    [SerializeField] private float hp;
    
    private PlayerController playerController;
    private Rigidbody[] rigidbodies;
    private LineOfSightTrump lineOfSight; // Referencia al script de disparo
    
    [SerializeField] private Animator animator;
    
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        lineOfSight = GetComponent<LineOfSightTrump>(); // Obtener referencia

        SetEnabled(false);
    }

    private void Update()
    {
        if (playerController != null)
        {
            Vector3 direction = playerController.transform.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }
    
    void SetEnabled(bool enabled)
    {
        bool isKinematic = !enabled;
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = isKinematic;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        animator.enabled = !enabled;
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died");

        // Notificar a LineOfSightTrump que debe detenerse
        if (lineOfSight != null)
        {
            lineOfSight.Die();
        }

        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;
        animator.enabled = false;
        SetEnabled(true);

        // Desactivar todos los scripts menos este
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }
    }
}
