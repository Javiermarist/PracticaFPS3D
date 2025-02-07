using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LineOfSight : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float detectionDelay = 0.5f;
    [Range(0, 360)]
    [SerializeField] private int visionAngle = 90;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float shootInterval = 1f;

    private Collider playerCollider;
    private SphereCollider detectionCollider;
    private Coroutine detectPlayerCoroutine;
    private Coroutine shootCoroutine;
    private Security security;

    private void Awake()
    {
        detectionCollider = GetComponent<SphereCollider>();
        security = GetComponentInParent<Security>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected by " + gameObject.name);
            target = other.gameObject;
            playerCollider = other;

            if (detectPlayerCoroutine == null)
                detectPlayerCoroutine = StartCoroutine(DetectPlayer());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player lost by " + gameObject.name);
            target = null;

            if (detectPlayerCoroutine != null)
            {
                StopCoroutine(detectPlayerCoroutine);
                detectPlayerCoroutine = null;
            }

            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
                shootCoroutine = null;
            }

            if (security != null)
            {
                security.SearchNearPlayer(other.transform.position);
            }
        }
    }

    IEnumerator DetectPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(detectionDelay);

            Vector3[] points = GetBoundingPoints(playerCollider.bounds);
            int pointsHidden = 0;

            foreach (Vector3 point in points)
            {
                Vector3 targetDirection = point - transform.position;
                float targetDistance = Vector3.Distance(transform.position, point);
                float targetAngle = Vector3.Angle(targetDirection, transform.forward);

                if (IsPointCovered(targetDirection, targetDistance) || targetAngle > visionAngle)
                    ++pointsHidden;
            }

            if (pointsHidden >= points.Length)
            {
                Debug.Log("Player is hidden");

                if (shootCoroutine != null)
                {
                    StopCoroutine(shootCoroutine);
                    shootCoroutine = null;
                }
            }
            else
            {
                Debug.Log("Player is visible");

                if (shootCoroutine == null)
                    shootCoroutine = StartCoroutine(ShootAtPlayer());
            }
        }
    }

    private bool IsPointCovered(Vector3 targetDirection, float targetDistance)
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, targetDirection, detectionCollider.radius);
        Debug.DrawRay(transform.position, targetDirection, Color.red, 1f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Cover"))
            {
                float coverDistance = Vector3.Distance(transform.position, hit.point);

                if (coverDistance < targetDistance)
                    return true;
            }
        }
        return false;
    }

    private Vector3[] GetBoundingPoints(Bounds bounds)
    {
        return new Vector3[]
        {
            bounds.min, bounds.max,
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z)
        };
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
            bullet.GetComponent<Rigidbody>().linearVelocity = (target.transform.position - bulletSpawnPoint.position).normalized * 20f;
        }
    }
}
