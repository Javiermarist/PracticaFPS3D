using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LineOfSight : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float detection_delay;
    [Range(0, 360)]
    [SerializeField] private int visionAngle;
    [SerializeField] private GameObject bulletPrefab; // Reference to the bullet prefab
    [SerializeField] private Transform bulletSpawnPoint; // Reference to the bullet spawn point
    [SerializeField] private float shootInterval = 1f; // Interval between shots

    private Collider player_collider;
    private SphereCollider detection_collider;
    private Coroutine detect_player;
    private Coroutine shootCoroutine;

    private void Awake() => detection_collider = this.GetComponent<SphereCollider>();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player on range");
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
            StopCoroutine(detect_player);
            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
            }
            Debug.Log("player out of range of " + gameObject.name);
        }
    }

    IEnumerator DetectPlayer()
    {
        while (true)
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
                StopCoroutine(shootCoroutine);
            }
            else
            {
                Debug.Log("player is visible");
                StartCoroutine(ShootAtPlayer());
            }
        }
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
            new Vector3( bounds.min.x, bounds.min.y, bounds.max.z ),
            new Vector3( bounds.min.x, bounds.max.y, bounds.min.z ),
            new Vector3( bounds.max.x, bounds.min.y, bounds.min.z ),
            new Vector3( bounds.min.x, bounds.max.y, bounds.max.z ),
            new Vector3( bounds.max.x, bounds.min.y, bounds.max.z ),
            new Vector3( bounds.max.x, bounds.max.y, bounds.min.z )
        };

        return bounding_points;
    }

    private IEnumerator ShootAtPlayer()
    {
        while (true)
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
            bullet.transform.rotation = Quaternion.LookRotation(direction);
            bullet.GetComponent<Rigidbody>().linearVelocity = direction * 20f; // Ajusta la velocidad según sea necesario
        }
    }
}