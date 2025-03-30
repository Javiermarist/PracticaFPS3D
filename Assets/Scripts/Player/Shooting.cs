using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public Camera playerCamera;
    public AudioSource shootSound;
    
    public GameObject AK47; // Referencia al objeto del AK-47
    public GameObject Glock; // Referencia al objeto de la pistola
    public float defaultFireRate = 0.5f; // Fire rate normal
    public float akFireRate = 0.2f; // Fire rate más rápido para el AK-47

    private float fireRate;
    private float nextFireTime = 0f;

    // Variables para el retroceso
    public float recoilForce = 1f; // Fuerza del retroceso
    public float recoilDuration = 0.1f; // Duración del retroceso

    // Variables específicas para cada arma
    public float recoilForceAK = 2f; // Fuerza del retroceso del AK-47
    public float recoilForceGlock = 1f; // Fuerza del retroceso de la pistola

    private Vector3 originalCameraPosition;
    private Vector3 originalAKPosition; // Posición original del AK-47
    private Vector3 originalGlockPosition; // Posición original de la pistola

    void Start()
    {
        UpdateFireRate();
        originalCameraPosition = playerCamera.transform.localPosition; // Guardamos la posición original de la cámara
        if (AK47 != null) originalAKPosition = AK47.transform.localPosition; // Posición original del AK-47
        if (Glock != null) originalGlockPosition = Glock.transform.localPosition; // Posición original de la pistola
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

            // Aplicar retroceso
            StartCoroutine(ApplyRecoil());
            StartCoroutine(ApplyWeaponRecoil());
        }
        else
        {
            Debug.LogWarning("Falta asignar el prefab de la bala, el punto de disparo o la cámara del jugador.");
        }
    }

    // Coroutine para aplicar el retroceso de la cámara
    IEnumerator ApplyRecoil()
    {
        float elapsedTime = 0f;
        Vector3 recoilPosition = playerCamera.transform.localPosition - new Vector3(0, 0, recoilForce);

        // Movimiento hacia atrás por el retroceso de la cámara
        while (elapsedTime < recoilDuration)
        {
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, recoilPosition, elapsedTime / recoilDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Vuelve a la posición original de la cámara
        elapsedTime = 0f;
        while (elapsedTime < recoilDuration)
        {
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, originalCameraPosition, elapsedTime / recoilDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // Coroutine para aplicar retroceso en el arma
    IEnumerator ApplyWeaponRecoil()
    {
        float elapsedTime = 0f;
        Vector3 recoilPosition;

        // Determina el retroceso dependiendo del arma activa
        if (AK47 != null && AK47.activeSelf)
        {
            recoilPosition = originalAKPosition - new Vector3(0, 0, recoilForceAK); // Retroceso del AK-47
        }
        else if (Glock != null && Glock.activeSelf)
        {
            recoilPosition = originalGlockPosition - new Vector3(0, 0, recoilForceGlock); // Retroceso de la pistola
        }
        else
        {
            yield break; // No se aplica retroceso si no hay arma activa
        }

        // Movimiento hacia atrás por el retroceso del arma
        while (elapsedTime < recoilDuration)
        {
            if (AK47 != null && AK47.activeSelf)
                AK47.transform.localPosition = Vector3.Lerp(AK47.transform.localPosition, recoilPosition, elapsedTime / recoilDuration);
            if (Glock != null && Glock.activeSelf)
                Glock.transform.localPosition = Vector3.Lerp(Glock.transform.localPosition, recoilPosition, elapsedTime / recoilDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Vuelve a la posición original del arma
        elapsedTime = 0f;
        while (elapsedTime < recoilDuration)
        {
            if (AK47 != null && AK47.activeSelf)
                AK47.transform.localPosition = Vector3.Lerp(AK47.transform.localPosition, originalAKPosition, elapsedTime / recoilDuration);
            if (Glock != null && Glock.activeSelf)
                Glock.transform.localPosition = Vector3.Lerp(Glock.transform.localPosition, originalGlockPosition, elapsedTime / recoilDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
