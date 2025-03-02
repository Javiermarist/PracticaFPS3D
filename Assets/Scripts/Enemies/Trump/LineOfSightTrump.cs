using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class LineOfSightTrump : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float detection_delay;
    [Range(0, 360)]
    [SerializeField] private int visionAngle;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float shootInterval = 1f;

    private Collider player_collider;
    private SphereCollider detection_collider;
    private Coroutine detect_player;
    private Coroutine shootCoroutine;
    private bool isDead = false; // Nueva variable

    private void Awake() => detection_collider = this.GetComponent<SphereCollider>();

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return; // No hacer nada si está muerto

        if (other.CompareTag("Player"))
        {
            Debug.Log("player on range " + gameObject.name);
            target = other.gameObject;
            detect_player = StartCoroutine(DetectPlayer());
            player_collider = other;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = null;
            if (detect_player != null)
            {
                StopCoroutine(detect_player);
                detect_player = null;
            }
            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
                shootCoroutine = null;
            }
            Debug.Log("player out of range of " + gameObject.name);
        }
    }

    IEnumerator DetectPlayer()
    {
        while (!isDead) // Solo sigue si no está muerto
        {
            yield return new WaitForSeconds(detection_delay);

            Vector3[] points = GetBoundingPoints(player_collider.bounds);
            int points_hidden = 0;

            foreach (Vector3 point in points)
            {
                Vector3 target_direction = point - this.transform.position;
                float target_distance = Vector3.Distance(this.transform.position, point);
                float target_angle = Vector3.Angle(target_direction, this.transform.forward);

                if (IsPointCovered(target_direction, target_distance) || target_angle > visionAngle)
                    ++points_hidden;
            }

            if (points_hidden >= points.Length)
            {
                Debug.Log("player is hidden");
                if (shootCoroutine != null)
                {
                    StopCoroutine(shootCoroutine);
                    shootCoroutine = null;
                }
            }
            else
            {
                Debug.Log("player is visible");
                if (shootCoroutine == null)
                {
                    shootCoroutine = StartCoroutine(ShootAtPlayer());
                }
            }
        }
    }

    private IEnumerator ShootAtPlayer()
    {
        while (!isDead) // No dispara si está muerto
        {
            yield return new WaitForSeconds(shootInterval);
            Shoot();
        }
    }

    private void Shoot()
    {
        if (target != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            Vector3 direction = (target.transform.position - bulletSpawnPoint.position).normalized;
            //bullet.transform.rotation = Quaternion.LookRotation(direction);
            bullet.GetComponent<Rigidbody>().linearVelocity = direction * 20f;
        }
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " has died");
        isDead = true;

        if (detect_player != null)
        {
            StopCoroutine(detect_player);
            detect_player = null;
        }
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }

        this.enabled = false; // Desactivar el script
    }

    private bool IsPointCovered(Vector3 target_direction, float target_distance)
    {
        RaycastHit[] hits = Physics.RaycastAll(this.transform.position, target_direction, detection_collider.radius);
        Debug.DrawRay(this.transform.position, target_direction, Color.red, 1f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Cover"))
            {
                float cover_distance = Vector3.Distance(this.transform.position, hit.point);
                if (cover_distance < target_distance)
                    return true;
            }
        }
        return false;
    }

    private Vector3[] GetBoundingPoints(Bounds bounds)
    {
        Vector3[] bounding_points =
        {
            bounds.min,
            bounds.max,
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z)
        };

        return bounding_points;
    }
}
