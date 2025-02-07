using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Behaviour : MonoBehaviour
{
    public enum NPCState { Patrol, Alert, Attack }
    public NPCState currentState = NPCState.Patrol;

    [Header("Patrol Settings")]
    [SerializeField] private Transform path;
    [SerializeField] private int childrenIndex = 0;

    [Header("Player Tracking")]
    [SerializeField] private GameObject player;
    [SerializeField] private float playerDetectionRange = 10f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float angleVision = 45f;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float fireRate = 1.5f;

    private NavMeshAgent agent;
    private bool isWaiting = false;
    private bool isShooting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetNextPatrolPoint();
    }

    void Update()
    {
        switch (currentState)
        {
            case NPCState.Patrol:
                Patrol();
                break;
            case NPCState.Alert:
                Alert();
                break;
            case NPCState.Attack:
                Attack();
                break;
        }
    }

    private void Patrol()
    {
        agent.isStopped = false;
        if (!isWaiting && Vector3.Distance(transform.position, agent.destination) < 1f)
        {
            StartCoroutine(WaitAndMoveToNextPoint());
        }
        DetectPlayer();
    }

    private void Alert()
    {
        agent.isStopped = true; // NPC se detiene

        // 🔹 Ahora gira hacia el jugador
        RotateTowards(player.transform.position);

        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            currentState = NPCState.Attack; // Cambia al estado Attack cuando el jugador está cerca
        }
        else if (Vector3.Distance(transform.position, player.transform.position) > playerDetectionRange)
        {
            currentState = NPCState.Patrol;
            isShooting = false;
            SetNextPatrolPoint();
        }
    }

    private void Attack()
    {
        agent.isStopped = true;
        
        RotateTowards(player.transform.position);

        if (!isShooting)
        {
            isShooting = true;
            StartCoroutine(ShootAtPlayer());
        }

        if (Vector3.Distance(transform.position, player.transform.position) > attackRange)
        {
            currentState = NPCState.Alert;
            isShooting = false;
        }
    }

    private IEnumerator WaitAndMoveToNextPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(2f);

        childrenIndex = (childrenIndex + 1) % path.childCount;
        SetNextPatrolPoint();

        isWaiting = false;
    }

    private void SetNextPatrolPoint()
    {
        agent.SetDestination(path.GetChild(childrenIndex).position);
    }

    private void DetectPlayer()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;

        if (Vector3.Distance(transform.position, player.transform.position) < playerDetectionRange &&
            Vector3.Angle(transform.forward, directionToPlayer) < angleVision)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, playerDetectionRange))
            {
                if (hit.collider.gameObject == player)
                {
                    currentState = NPCState.Attack;
                }
            }
        }
    }

    private IEnumerator ShootAtPlayer()
    {
        while (currentState == NPCState.Attack)
        {
            yield return new WaitForSeconds(fireRate);
            Shoot();
        }
        isShooting = false;
    }

    private void Shoot()
    {
        if (player == null) return;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            Vector3 direction = (player.transform.position - bulletSpawnPoint.position).normalized;
            bulletRb.linearVelocity = direction * 20f; // Asegúrate de usar "velocity" en vez de "linearVelocity"
        }
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Evita que el NPC gire en el eje Y de forma incorrecta
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);

        Vector3 forward = transform.forward * playerDetectionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, -angleVision / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, angleVision / 2, 0) * forward;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
