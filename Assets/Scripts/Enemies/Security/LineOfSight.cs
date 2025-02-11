using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LineOfSight : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float detectionDelay = 0.5f;
    [Range(0, 360)]
    [SerializeField] private int visionAngle;

    private Collider playerCollider;
    private SphereCollider detectionCollider;
    private Coroutine detectPlayerCoroutine;
    private EnemyState enemyState;

    private void Awake()
    {
        detectionCollider = GetComponent<SphereCollider>();
        enemyState = GetComponentInParent<EnemyState>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected by " + gameObject.name);
            target = other.gameObject;
            detectPlayerCoroutine = StartCoroutine(DetectPlayer());
            playerCollider = other;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player out of range of " + gameObject.name);
            target = null;
            StopCoroutine(detectPlayerCoroutine);

            if (enemyState != null && enemyState.state != EnemyState.State.Alert)
                enemyState.SetState(EnemyState.State.Patrol);
        }
    }

    IEnumerator DetectPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(detectionDelay);
        
            Vector3[] points = GetBoundingPoints(playerCollider.bounds);   
        
            int points_hidden = 0;

            foreach (Vector3 point in points)
            {
                Vector3 targetDirection = point - transform.position;
                float targetDistance = Vector3.Distance(transform.position, point);
                float targetAngle = Vector3.Angle(targetDirection, transform.forward);

                if (IsPointCovered(targetDirection, targetDistance) || targetAngle > visionAngle)
                    ++points_hidden;
            }
        
            if (points_hidden >= points.Length)
            {
                Debug.Log("Player is hidden");

                if (enemyState != null && enemyState.state == EnemyState.State.Attack)
                {
                    enemyState.SetState(EnemyState.State.Alert, playerCollider.gameObject);  
                }
                else if (enemyState != null && enemyState.state != EnemyState.State.Alert)
                {
                    enemyState.SetState(EnemyState.State.Patrol); 
                }
            }
            else
            {
                Debug.Log("Player is visible");

                if (enemyState != null && (enemyState.state == EnemyState.State.Alert || enemyState.state == EnemyState.State.Patrol))
                {
                    enemyState.SetState(EnemyState.State.Attack, playerCollider.gameObject);
                }
            }
        }
    }

    private bool IsPointCovered(Vector3 target_direction, float target_distance)
    {
        RaycastHit[] hits = Physics.RaycastAll(this.transform.position, target_direction, detectionCollider.radius);
        Debug.DrawRay(transform.position, target_direction * detectionCollider.radius, Color.red, 0.1f);

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
}
