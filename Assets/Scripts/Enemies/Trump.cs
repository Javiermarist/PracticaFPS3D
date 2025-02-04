using System;
using UnityEngine;

public class Trump : MonoBehaviour
{
    [SerializeField] private float hp;
    
    private PlayerController playerController;
    [SerializeField] private Animator animator; // Reference to the Animator component
    
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
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

    private void Die()
    {
        Debug.Log("Trump died");
        animator.SetTrigger("Die");
        GetComponentInChildren<MeshRenderer>().enabled = false;
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
    }
}