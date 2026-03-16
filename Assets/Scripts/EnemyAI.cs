using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Game Settings")]
    public GameSettings gameSettings;
    // Basic Settings
    public Transform playerTarget;
    private bool playerInRange;
    public float detectionRange = 15f; // range for spray attack
    public float rotationSpeed = 5f;
    public float fleeRange = 8f;
    public float speed = 5f;
    private const float gravity = -9.81f;
    private Vector3 gravityVector = new Vector3(0, gravity, 0);
    public NavMeshAgent agent;
    private NavMeshPath path;
    // Spray Settings
    public float damagePerSecond = 0.5f;    // damage every second
    public ParticleSystem sprayEffect;
    public Transform firePoint;
    private float damageAccumulator = 0f;
    private float pathTimer = 0f;
    private float shootAudioTimer = 0f;  // Prevents audio spam

    // save the baseline stats so it can be multiplied
    private float baseSpeed;
    private float baseDamage;

    void Start()
    {
        // Save the original stats
        baseSpeed = speed;
        baseDamage = damagePerSecond;

        // STANDARD DIFFICULTY SCALING
        if (gameSettings != null)
        {
            if (gameSettings.difficulty == "Hard")
            {
                baseSpeed = 4f;
                baseDamage = 1.5f;  // 1.5x damage
            }
            else if (gameSettings.difficulty == "Super Hard")
            {
                baseSpeed = 5f;
                baseDamage = 3f;  // 3x damage
            }
        }

        // Apply standard scaling so normal modes still work
        speed = baseSpeed;
        damagePerSecond = baseDamage;

        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTarget = playerObj.transform;
        }
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = speed;
        }
        path = new NavMeshPath();
        if (sprayEffect != null) sprayEffect.Stop();
    }

    void Update()
    {
        if (playerTarget == null || agent == null) return;

        // TRADE SHOW SCALING
        if (ManageUI.Instance != null && ManageUI.Instance.isTradeShow)
        {
            int threat = ManageUI.Instance.wantedLevel;

            // Speed increases linearly per threat level, damage multiplies
            agent.speed = baseSpeed + (threat * 0.6f);
            damagePerSecond = baseDamage * threat;
        }

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

        UpdateNavPath(startHit.position, endHit.position);
    }

    void ChasePlayer()
    {
        if (!NavMesh.SamplePosition(agent.transform.position, out NavMeshHit startHit, 5f, agent.areaMask)) return;
        if (!NavMesh.SamplePosition(playerTarget.position, out NavMeshHit endHit, 20f, agent.areaMask)) return;

        UpdateNavPath(startHit.position, endHit.position);
    }

    // Centralizes the pathing logic
    void UpdateNavPath(Vector3 startPosition, Vector3 endPosition)
    {
        NavMesh.CalculatePath(
            startPosition,
            endPosition,
            new NavMeshQueryFilter { agentTypeID = agent.agentTypeID, areaMask = agent.areaMask },
            path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetPath(path);
                }
                else
                {
                    Debug.LogWarning($"EnemyAI: path status {path.status}", this);
                }
        if (sprayEffect != null && sprayEffect.isPlaying)
        {
            sprayEffect.Stop();
        }
    }

    void aimAtPlayer()
    {
        Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;
        directionToPlayer.y = 0; // Keep only horizontal rotation

        if (directionToPlayer == Vector3.zero) return; // Avoid errors

        // Determine target rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Smoothly rotate towards player
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

        if (agent != null)
        {
            agent.isStopped = true; // Stop moving
        }

        // Visuals: Turn on the water spray if it's not already on
        if (sprayEffect != null && !sprayEffect.isPlaying)
        {
            sprayEffect.Play();
        }

        // Damage Logic: Use a Raycast to see if player is being hit
        RaycastHit hit;
        // Cast a ray from firePoint, going forward, for the length of range
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, detectionRange))
        {
            // Was the player hit
            PlayerHealth playerHealth = hit.collider.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Accumulate Damage
                damageAccumulator += damagePerSecond * Time.deltaTime;

                // Whenever 1 full point of damage is accumulated, deal it
                if (damageAccumulator >= 1f)
                {
                    playerHealth.TakeDamage(1); // Deal 1 damage
                    damageAccumulator -= 1f;    // Keep the remainder for next frame
                }
            }
        }
    }
}