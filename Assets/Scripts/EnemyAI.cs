using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Basic Settings
    public Transform playerTarget;
    private bool playerInRange;
    public float detectionRange = 15f; // range for spray attack
    public float rotationSpeed = 5f;
    public float speed = 5f;
    private const float gravity = -9.81f;
    private Vector3 gravityVector = new Vector3(0, gravity, 0);
    public NavMeshAgent agent;
    private NavMeshPath path;
    // Spray Settings
    public int damagePerSecond = 5;    // damage every second
    public ParticleSystem sprayEffect;
    public Transform firePoint;        // Where the spray comes from
    private float damageAccumulator = 0f; // Stores partial damage
    private float elapsed = 0f; // Timer for pathfinding updates

    void Start()
    {
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
        playerInRange = Physics.CheckSphere(transform.position, detectionRange, LayerMask.GetMask("player"));
        if (playerInRange)
        {
            aimAtPlayer();
            SprayAttack();
        }
        else
        {
            aimAtPlayer();
            findPlayer();
        }
    }

    void findPlayer()
    {
        if (agent != null && playerTarget != null)
        {
            agent.isStopped = false;


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
                    agent.areaMask,
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
        // Play a shoot sound effect (Maybe turn this off if it plays a bunch)
        AudioManager.Instance.playAudio(AudioManager.Instance.enemyShoot);
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