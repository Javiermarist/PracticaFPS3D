using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Security : MonoBehaviour
{
    [SerializeField] private float hp = 100f;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float searchDuration = 10f;
    [SerializeField] private GameObject bloodEffectPrefab; // Prefab de sangre

    private int currentWaypointIndex = 0;
    private PlayerController playerController;
    private NavMeshAgent agent;
    private Rigidbody[] rigidbodies;
    private Animator animator;
    private LineOfSight lineOfSight;
    private EnemyState enemyState;
    private bool isSearching = false;
    public bool isDead = false;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        lineOfSight = GetComponent<LineOfSight>();
        enemyState = GetComponent<EnemyState>();

        SetEnabled(false);
        GoToNextWaypoint();
    }

    private void Update()
    {
        if (isDead) return;

        if (enemyState != null && enemyState.state == EnemyState.State.Attack && playerController != null)
        {
            Vector3 direction = playerController.transform.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else if (agent.velocity.sqrMagnitude > 0.1f)
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
                Vector3 hitPoint = other.contacts[0].point; // Obtiene el punto de impacto
                TakeDamage(bullet.damage, hitPoint);
            }
        }
    }

    public void TakeDamage(float damage, Vector3 hitPosition)
    {
        hp -= damage;

        // Rotación aleatoria en cualquier dirección
        Quaternion randomRotation = Quaternion.Euler(Random.Range(-30f, 30f), Random.Range(0f, 360f), Random.Range(-30f, 30f));
        
        // Instanciar sangre en la posición exacta del impacto con rotación aleatoria
        if (bloodEffectPrefab != null)
        {
            Instantiate(bloodEffectPrefab, hitPosition, randomRotation);
        }

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

    public void Die()
    {
        Debug.Log("Security died");
        isDead = true;

        // Desactivar el modelo, collider, y animaciones del enemigo
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;
        animator.enabled = false;
        SetEnabled(true);

        // Deshabilitar el agente de navegación
        gameObject.GetComponent<NavMeshAgent>().enabled = false;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // Detener todas las corrutinas activas del enemigo
        if (lineOfSight != null)
        {
            lineOfSight.StopAllCoroutines();
            lineOfSight.enabled = false;
        }

        if (enemyState != null)
        {
            enemyState.SetState(EnemyState.State.Patrol);  // Cambiar a estado de patrulla si es necesario
            enemyState.StopAllCoroutines();  // Detener todas las corrutinas activas
            enemyState.enabled = false; // Deshabilitar el script EnemyState
        }

        // Detener cualquier corrutina adicional en otros scripts si es necesario
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;  // Deshabilitar todos los scripts
        }
    }
}
