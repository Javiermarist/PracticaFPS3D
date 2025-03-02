// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// public class Shooting : MonoBehaviour
// {
//     public GameObject bulletPrefab;
//     public Transform firePoint;
//     public float bulletForce;
//
//     void Update()
//     {
//         if (Input.GetMouseButtonDown(0))
//         {
//             Shoot();
//         }
//     }
//     
//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Bullet"))
//         {
//             Destroy(other.gameObject);
//         }
//     }
//
//     void Shoot()
//     {
//         if (bulletPrefab != null && firePoint != null)
//         {
//             GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
//             Rigidbody rb = bullet.GetComponent<Rigidbody>();
//             if (rb != null)
//             {
//                 rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
//             }
//             
//             Destroy(bullet, 3f);
//         }
//         else
//         {
//             Debug.LogWarning("Falta asignar el prefab de la bala o el punto de disparo.");
//         }
//     }
// }

using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public Camera playerCamera; // Asigna aquí la cámara del jugador

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null && playerCamera != null)
        {
            // Definir un LayerMask para ignorar la capa de los enemigos (debe estar en el índice correcto)
            int layerMask = ~LayerMask.GetMask("Enemy"); // El ~ invierte la máscara para ignorarla

            // Lanzar un raycast desde el centro de la cámara
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) // Aplicamos la máscara aquí
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(1000);
            }

            // Calcular la dirección hacia el punto objetivo
            Vector3 direction = (targetPoint - firePoint.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Quaternion correctedRotation = lookRotation * Quaternion.Euler(90f, 0f, 0f);

            // Instanciar y lanzar la bala
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, correctedRotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(direction * bulletForce, ForceMode.Impulse);
            }

            Destroy(bullet, 3f);
        }
        else
        {
            Debug.LogWarning("Falta asignar el prefab de la bala, el punto de disparo o la cámara del jugador.");
        }
    }
}
