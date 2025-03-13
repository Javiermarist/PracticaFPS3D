using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public Camera playerCamera;
    public AudioSource shootSound;
    
    public GameObject AK47; // Referencia al objeto del AK-47
    public float defaultFireRate = 0.5f; // Fire rate normal
    public float akFireRate = 0.2f; // Fire rate más rápido para el AK-47

    private float fireRate;
    private float nextFireTime = 0f;

    void Start()
    {
        UpdateFireRate();
    }

    void Update()
    {
        UpdateFireRate(); // Asegura que el fireRate cambie dinámicamente

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void UpdateFireRate()
    {
        // Si el AK-47 está activo, usar su fire rate, de lo contrario, usar el normal
        fireRate = (AK47 != null && AK47.activeSelf) ? akFireRate : defaultFireRate;
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null && playerCamera != null)
        {
            int layerMask = ~LayerMask.GetMask("Enemy");

            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            Vector3 targetPoint = Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) ? hit.point : ray.GetPoint(1000);
            Vector3 direction = (targetPoint - firePoint.position).normalized;
            Quaternion correctedRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, correctedRotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(direction * bulletForce, ForceMode.Impulse);
            }

            // Reproducir sonido de disparo
            if (shootSound != null)
            {
                shootSound.Play();
            }

            Destroy(bullet, 3f);
        }
        else
        {
            Debug.LogWarning("Falta asignar el prefab de la bala, el punto de disparo o la cámara del jugador.");
        }
    }
}
