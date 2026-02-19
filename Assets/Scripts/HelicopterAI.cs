using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class HelicopterAI : MonoBehaviour
{

    public Transform playerTarget;
    private bool playerInRange;
    private bool playerInSight;
    public float fireRange = 60f;
    public NavMeshAgent agent;
    private NavMeshPath path;
    private float elapsed = 0f; // Timer for pathfinding updates
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireCooldown = 0.02f;
    private float lastFireTime = 0f;


    void Start()
    {
        if (playerTarget == null)
        {
            GameObject playerHeadObj = GameObject.FindGameObjectWithTag("PlayerHead");
            if (playerHeadObj != null) playerTarget = playerHeadObj.transform;
        }
        agent = GetComponent<NavMeshAgent>();
        agent.areaMask = 1 << NavMesh.GetAreaFromName("Flyable");
        path = new NavMeshPath();
        agent.stoppingDistance = fireRange;
    }

    // Update is called once per frame
    void Update()
    {
        playerInRange = Physics.CheckSphere(transform.position, fireRange, LayerMask.GetMask("player"));
        playerInSight = Physics.Raycast(transform.position, playerTarget.position - transform.position, out RaycastHit hit, fireRange, LayerMask.GetMask("player", "Default", "Building"));

        if (playerInRange && playerInSight && hit.collider.CompareTag("Player"))
        {
            agent.stoppingDistance = fireRange;
            Shootplayer();
        }
        else if (playerInRange && !hit.collider.CompareTag("Player"))
        {
            agent.stoppingDistance = 0f;
            FindPlayer();
        }
        else
        {
            agent.stoppingDistance = fireRange;
            GoToPlayer();
        }

    }

    void FindPlayer()
    {
        // If player is not in sight but is in range, move towards last known position
        elapsed += Time.deltaTime;
        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;
            if (!NavMesh.SamplePosition(agent.transform.position, out NavMeshHit startHit, 2f, agent.areaMask))
            {
                Debug.LogWarning("EnemyAI: agent is not on the NavMesh.", this);
                return;
            }

            if (!NavMesh.SamplePosition(playerTarget.position, out NavMeshHit endHit, 20f, agent.areaMask))
            {
                Debug.LogWarning("EnemyAI: target is not on the NavMesh.", this);
                return;
            }
            NavMesh.CalculatePath(
                startHit.position,
                endHit.position,
                new NavMeshQueryFilter
                {
                    agentTypeID = agent.agentTypeID,
                    areaMask = agent.areaMask
                },
                path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetPath(path);
            }
            else
            {
                Debug.LogWarning($"EnemyAI: path status {path.status}", this);
            }
        }
    }

    void GoToPlayer()
    {
        elapsed += Time.deltaTime;
        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;
            if (!NavMesh.SamplePosition(agent.transform.position, out NavMeshHit startHit, 2f, agent.areaMask))
            {
                Debug.LogWarning("EnemyAI: agent is not on the NavMesh.", this);
                return;
            }

            if (!NavMesh.SamplePosition(playerTarget.position, out NavMeshHit endHit, 20f, agent.areaMask))
            {
                Debug.LogWarning("EnemyAI: target is not on the NavMesh.", this);
                return;
            }
            NavMesh.CalculatePath(
                startHit.position,
                endHit.position,
                new NavMeshQueryFilter
                {
                    agentTypeID = agent.agentTypeID,
                    areaMask = agent.areaMask
                },
                path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetPath(path);
            }
            else
            {
                Debug.LogWarning($"EnemyAI: path status {path.status}", this);
            }
        }
    }

    void Shootplayer()
    {
        if (Time.time - lastFireTime < fireCooldown)
        {
            return;
        }
        lastFireTime = Time.time;
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projScript = projectile.GetComponent<Projectile>();
            if (projScript != null)
            {
                projScript.target = playerTarget;
            }
        }
    }
}
