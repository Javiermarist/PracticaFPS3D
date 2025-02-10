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
    private NavMeshAgent agent; // Referencia al NavMeshAgent

    private Vector3 lastKnownPosition;  // Para almacenar la última posición conocida del jugador
    private Coroutine alertCoroutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); // Obtiene el agente si existe
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
            StopMovement(); // Detener movimiento al atacar
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
                ResumeMovement(); // Reanudar movimiento en patrulla
            }
            else if (newState == State.Alert && player != null)
            {
                lastKnownPosition = player.transform.position;  // Guardar la última posición conocida del jugador

                // Si ya hay una Coroutine de alerta activa, la detenemos
                if (alertCoroutine != null)
                {
                    StopCoroutine(alertCoroutine);
                    alertCoroutine = null;
                }

                // Iniciar nueva búsqueda en posiciones cercanas
                alertCoroutine = StartCoroutine(SearchAround(lastKnownPosition));  // Buscar en posiciones cercanas
            }
        }
    }

    private void StopMovement()
    {
        if (agent != null)
        {
            agent.isStopped = true; // Detener movimiento
        }
    }

    private void ResumeMovement()
    {
        if (agent != null)
        {
            agent.isStopped = false; // Reanudar movimiento
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
            // Calcular la dirección hacia el jugador desde la posición de disparo
            Vector3 shootDirection = (target.transform.position - bulletSpawnPoint.position).normalized;

            // Instanciar la bala
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

            // Obtener el Rigidbody de la bala y aplicarle la velocidad en la dirección calculada
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = shootDirection * 20f;  // Ajusta la velocidad de la bala según sea necesario
            }
            else
            {
                Debug.LogError("La bala no tiene un Rigidbody asignado.");
            }
        }
    }

    private IEnumerator SearchAround(Vector3 searchCenter)
    {
        Debug.Log("Estado: Alerta");  // Mensaje de depuración
        float searchDuration = 5f;  // Tiempo total que el enemigo buscará
        float timeElapsed = 0f;

        // Asegúrate de que el agente no esté detenido
        agent.isStopped = false; // Habilitamos el movimiento

        // Durante 5 segundos, el enemigo buscará en diferentes puntos cercanos
        while (timeElapsed < searchDuration)
        {
            // Definir el número de puntos a buscar
            int numberOfSearchPoints = 3;  // Puedes ajustar el número de puntos

            for (int i = 0; i < numberOfSearchPoints; i++)
            {
                // Buscar en posiciones aleatorias cerca de la última posición conocida
                Vector3 randomSearchPosition = searchCenter + new Vector3(
                    Random.Range(-5f, 5f), // Rango de búsqueda en el eje X
                    0f, // Si el movimiento es solo en un plano 2D, la posición en Y es constante
                    Random.Range(-5f, 5f)  // Rango de búsqueda en el eje Z
                );

                // Mover al enemigo a la posición aleatoria
                agent.SetDestination(randomSearchPosition);
                Debug.Log("Buscando en: " + randomSearchPosition); // Mensaje de depuración

                // Esperar un poco antes de buscar otra posición aleatoria
                yield return new WaitForSeconds(1f);  // Espera entre cada búsqueda

                timeElapsed += 1f;  // Aumentar el tiempo transcurrido
                if (timeElapsed >= searchDuration) break;  // Si ya pasó el tiempo de búsqueda, salir del bucle
            }
        }

        // Después de buscar, volver al estado de patrullaje
        Debug.Log("Estado: Patrulla");  // Mensaje de depuración
        SetState(State.Patrol);  // Volver al estado de patrullaje
    }
}
