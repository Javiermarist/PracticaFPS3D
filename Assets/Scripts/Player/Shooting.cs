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
            // Lanzar un raycast desde el centro de la cámara
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                // Si el raycast no impacta, establecer un punto lejano en la dirección de la cámara
                targetPoint = ray.GetPoint(1000); // Puedes ajustar la distancia según tus necesidades
            }

            // Calcular la dirección hacia el punto objetivo
            Vector3 direction = (targetPoint - firePoint.position).normalized;

            // Calcular la rotación necesaria para que la bala apunte hacia el punto objetivo
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Aplicar una rotación adicional de -90 grados en el eje X para corregir la orientación del prefab
            Quaternion correctedRotation = lookRotation * Quaternion.Euler(90f, 0f, 0f);

            // Instanciar la bala en el firePoint con la rotación corregida
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, correctedRotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Aplicar fuerza a la bala en la dirección calculada
                rb.AddForce(direction * bulletForce, ForceMode.Impulse);
            }

            // Destruir la bala después de 3 segundos para evitar acumulación de objetos
            Destroy(bullet, 3f);
        }
        else
        {
            Debug.LogWarning("Falta asignar el prefab de la bala, el punto de disparo o la cámara del jugador.");
        }
    }
}
