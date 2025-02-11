using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyState : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Attack,
        Alert
    }

    public State state;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float shootInterval = 1f;

    private GameObject target;
    private Coroutine attackCoroutine;
    private NavMeshAgent agent;

    private Vector3 lastKnownPosition;
    private Coroutine alertCoroutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        state = State.Patrol;
    }

    public void SetState(State newState, GameObject player = null)
    {
        state = newState;

        if (newState == State.Attack && player != null)
        {
            target = player;
            StopMovement();
            if (attackCoroutine == null)
                attackCoroutine = StartCoroutine(AttackPlayer());
        }
        else
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }

            if (newState == State.Patrol)
            {
                ResumeMovement();
            }
            else if (newState == State.Alert && player != null)
            {
                lastKnownPosition = player.transform.position;
                
                if (alertCoroutine != null)
                {
                    StopCoroutine(alertCoroutine);
                    alertCoroutine = null;
                }
                
                alertCoroutine = StartCoroutine(SearchAround(lastKnownPosition));
            }
        }
    }

    private void StopMovement()
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }

    private void ResumeMovement()
    {
        if (agent != null)
        {
            agent.isStopped = false;
        }
        else
        {
            Debug.LogError("No se ha asignado un NavMeshAgent.");
        }
    }

    private IEnumerator AttackPlayer()
    {
        while (state == State.Attack && target != null)
        {
            yield return new WaitForSeconds(shootInterval);
            Shoot();
        }
    }

    private void Shoot()
    {
        if (target != null)
        {
            Vector3 shootDirection = (target.transform.position - bulletSpawnPoint.position).normalized;
            
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = shootDirection * 20f;
            }
            else
            {
                Debug.LogError("La bala no tiene un Rigidbody asignado.");
            }
        }
    }

    private IEnumerator SearchAround(Vector3 searchCenter)
    {
        Debug.Log("Estado: Alerta");
        float searchDuration = 5f;
        float timeElapsed = 0f;
        
        agent.isStopped = false;
        
        while (timeElapsed < searchDuration)
        {
            int numberOfSearchPoints = 3;

            for (int i = 0; i < numberOfSearchPoints; i++)
            {
                Vector3 randomSearchPosition = searchCenter + new Vector3(
                    Random.Range(-5f, 5f),
                    0f,
                    Random.Range(-5f, 5f)
                );
                
                agent.SetDestination(randomSearchPosition);
                Debug.Log("Buscando en: " + randomSearchPosition);
                
                yield return new WaitForSeconds(1.5f);

                timeElapsed += 1f;
                if (timeElapsed >= searchDuration) break;
            }
        }
        Debug.Log("Estado: Patrulla");
        SetState(State.Patrol);
    }
}
