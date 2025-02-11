using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Security : MonoBehaviour
{
    [SerializeField] private float hp = 100f;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float searchDuration = 10f;

    private int currentWaypointIndex = 0;
    private PlayerController playerController;
    private NavMeshAgent agent;
    private Rigidbody[] rigidbodies;
    private Animator animator;
    private LineOfSight lineOfSight;
    private EnemyState enemyState; // Referencia al script EnemyState
    private bool isSearching = false;
    private bool isDead = false;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        lineOfSight = GetComponent<LineOfSight>();
        enemyState = GetComponent<EnemyState>(); // Obtiene el componente EnemyState

        SetEnabled(false);
        GoToNextWaypoint();
    }

    private void Update()
    {
        if (isDead) return;

        if (enemyState != null && enemyState.state == EnemyState.State.Attack && playerController != null)
        {
            // Si está en estado de ataque, mirar al jugador
            Vector3 direction = playerController.transform.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else if (agent.velocity.sqrMagnitude > 0.1f) // Si no está atacando, mirar hacia adelante
        {
            Vector3 moveDirection = agent.velocity.normalized;
            moveDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isSearching)
        {
            GoToNextWaypoint();
        }
    }

    private void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    public void SearchNearPlayer(Vector3 lastPosition)
    {
        StartCoroutine(SearchForPlayer(lastPosition));
    }

    private IEnumerator SearchForPlayer(Vector3 lastPosition)
    {
        isSearching = true;
        float startTime = Time.time;

        while (Time.time - startTime < searchDuration)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
            agent.SetDestination(lastPosition + randomOffset);
            yield return new WaitForSeconds(3f);
        }

        isSearching = false;
        GoToNextWaypoint();
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

    void SetEnabled(bool enabled)
    {
        bool isKinematic = !enabled;
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = isKinematic;
            rb.mass = 10;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        animator.enabled = !enabled;
    }

    private void Die()
    {
        Debug.Log("Security died");
        isDead = true;

        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;

        animator.enabled = false;
        SetEnabled(true);

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (lineOfSight != null)
        {
            lineOfSight.StopAllCoroutines();
            lineOfSight.enabled = false;
        }

        if (enemyState != null)
        {
            enemyState.SetState(EnemyState.State.Patrol);
            enemyState.StopAllCoroutines();
            enemyState.enabled = false;
        }

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }
    }
}
