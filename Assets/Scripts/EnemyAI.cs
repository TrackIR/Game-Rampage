using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Game Settings")]
    public GameSettings gameSettings;

    // Basic Settings
    public Transform playerTarget;
    public float detectionRange = 15f;
    public float fleeRange = 8f;
    public float rotationSpeed = 5f;
    public float speed = 3f;

    private const float gravity = -9.81f;
    private Vector3 gravityVector = new Vector3(0, gravity, 0);

    public NavMeshAgent agent;
    private NavMeshPath path;

    // Spray Settings
    public float damagePerSecond = 0.5f;
    public ParticleSystem sprayEffect;
    public Transform firePoint;
    private float damageAccumulator = 0f;
    private float pathTimer = 0f;
    private float shootAudioTimer = 0f;  // Prevents audio spam

    void Start()
    {
        // DIFFICULTY SCALING
        if (gameSettings != null)
        {
            if (gameSettings.difficulty == "Hard")
            {
                speed = 4f;
                damagePerSecond = 1.0f;  // Double damage
            }
            else if (gameSettings.difficulty == "Super Hard")
            {
                speed = 5f;
                damagePerSecond = 2.0f;  // Quadruple damage
            }
        }

        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTarget = playerObj.transform;
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = speed;
            agent.updateRotation = false; // Disconnects agent rotation so we can manually point it
        }

        path = new NavMeshPath();
        if (sprayEffect != null) sprayEffect.Stop();

        // Force path calculation immediately on the exact frame they spawn
        pathTimer = 100f;
    }

    void Update()
    {
        if (playerTarget == null || agent == null) return;

        // Use 2D horizontal distance to prevent the giant robot's height from breaking the range checks
        Vector3 enemyFlatPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerFlatPos = new Vector3(playerTarget.position.x, 0, playerTarget.position.z);
        float distanceToPlayer = Vector3.Distance(enemyFlatPos, playerFlatPos);

        aimAtPlayer();

        pathTimer += Time.deltaTime;
        bool shouldUpdatePath = false;
        if (pathTimer > 0.5f)
        {
            shouldUpdatePath = true;
            pathTimer = 0f;
        }

        if (distanceToPlayer <= fleeRange)
        {
            // Player is too close, run away
            agent.isStopped = false;
            if (shouldUpdatePath) FleePlayer();

            SprayAttack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // Stand ground and attack
            agent.isStopped = true;
            SprayAttack();
        }
        else
        {
            // Chase player
            agent.isStopped = false;
            if (shouldUpdatePath) ChasePlayer();

            if (sprayEffect != null && sprayEffect.isPlaying)
            {
                sprayEffect.Stop();
            }
        }
    }

    void FleePlayer()
    {
        Vector3 dirAwayFromPlayer = (transform.position - playerTarget.position).normalized;
        dirAwayFromPlayer.y = 0;

        Vector3 fleeTarget = transform.position + (dirAwayFromPlayer * 10f);

        if (!NavMesh.SamplePosition(agent.transform.position, out NavMeshHit startHit, 5f, agent.areaMask)) return;
        if (!NavMesh.SamplePosition(fleeTarget, out NavMeshHit endHit, 10f, agent.areaMask)) return;

        NavMesh.CalculatePath(
            startHit.position,
            endHit.position,
            new NavMeshQueryFilter { agentTypeID = agent.agentTypeID, areaMask = agent.areaMask },
            path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetPath(path);
        }
    }

    void ChasePlayer()
    {
        if (!NavMesh.SamplePosition(agent.transform.position, out NavMeshHit startHit, 5f, agent.areaMask)) return;
        if (!NavMesh.SamplePosition(playerTarget.position, out NavMeshHit endHit, 20f, agent.areaMask)) return;

        NavMesh.CalculatePath(
            startHit.position,
            endHit.position,
            new NavMeshQueryFilter { agentTypeID = agent.agentTypeID, areaMask = agent.areaMask },
            path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetPath(path);
        }
    }

    void aimAtPlayer()
    {
        Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;
        directionToPlayer.y = 0;

        if (directionToPlayer == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void SprayAttack()
    {
        // Branch audio implementation with a small cooldown limit
        if (AudioManager.Instance != null && AudioManager.Instance.enemyShoot != null)
        {
            shootAudioTimer -= Time.deltaTime;
            if (shootAudioTimer <= 0f)
            {
                AudioManager.Instance.playAudio(AudioManager.Instance.enemyShoot);
                shootAudioTimer = 0.5f; // Play sound every half second
            }
        }

        if (sprayEffect != null && !sprayEffect.isPlaying)
        {
            sprayEffect.Play();
        }

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, detectionRange))
        {
            PlayerHealth playerHealth = hit.collider.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                damageAccumulator += damagePerSecond * Time.deltaTime;
                if (damageAccumulator >= 1f)
                {
                    playerHealth.TakeDamage(1);
                    damageAccumulator -= 1f;
                }
            }
        }
    }
}