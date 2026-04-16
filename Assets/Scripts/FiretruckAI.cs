using UnityEngine;
using UnityEngine.AI;

public class FiretruckAI : MonoBehaviour
{
    [Header("References")]
    public Transform playerTarget;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Turret & Spray Settings")]
    public Transform turret;
    public ParticleSystem sprayEffect;
    public Transform firePoint;
    public float detectionRange = 15f;
    public float rotationSpeed = 5f;
    public float damagePerSecond = 0.5f;
    private float damageAccumulator = 0f;

    private Quaternion currentTurretRotation;

    [Header("Movement Settings")]
    public float speed = 5f;
    private float elapsed = 0f;
    private NavMeshPath path;
    private bool playerInRange;

    [Header("Spawning Mechanics")]
    public GameObject normalEnemyPrefab;
    public Transform rearSpawnPoint;
    public float spawnInterval = 5f;
    private float spawnTimer;

    void Start()
    {
        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTarget = playerObj.transform;
        }
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (agent != null) agent.speed = speed;
        path = new NavMeshPath();
        if (sprayEffect != null) sprayEffect.Stop();

        // Initialize the rotation to wherever the turret starts
        if (turret != null) currentTurretRotation = turret.rotation;
    }

    void Update()
    {
        HandleSpawning();

        playerInRange = Physics.CheckSphere(transform.position, detectionRange, LayerMask.GetMask("player"));

        if (playerInRange)
        {
            if (agent.isOnNavMesh) agent.isStopped = true;
            if (animator != null) animator.SetBool("IsDriving", false);

            SprayAttack();
        }
        else
        {
            if (animator != null) animator.SetBool("IsDriving", true);
            findPlayer();
        }
    }

    void LateUpdate()
    {
        // Spin the visual Turret
        Vector3 directionToPlayer = (turret.position - playerTarget.position).normalized;
        directionToPlayer.y = 0;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            currentTurretRotation = Quaternion.Slerp(currentTurretRotation, targetRotation, rotationSpeed * Time.deltaTime);
            turret.rotation = currentTurretRotation;
        }

        // Force the mechanics to aim directly at the player
        if (firePoint != null)
        {
            firePoint.LookAt(playerTarget.position);

            // force the visual water particles to match the damage laser's rotation
            if (sprayEffect != null)
            {
                sprayEffect.transform.rotation = firePoint.rotation;
            }
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
                if (!NavMesh.SamplePosition(agent.transform.position, out NavMeshHit startHit, 2f, agent.areaMask)) return;
                if (!NavMesh.SamplePosition(playerTarget.position, out NavMeshHit endHit, 20f, agent.areaMask)) return;

                NavMesh.CalculatePath(
                    startHit.position,
                    endHit.position,
                    new NavMeshQueryFilter { agentTypeID = agent.agentTypeID, areaMask = agent.areaMask },
                    path
                );

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetPath(path);
                }
            }
        }

        if (sprayEffect != null && sprayEffect.isPlaying)
        {
            sprayEffect.Stop();
        }
    }

    void AimTurret()
    {
        // Spin the visual Turret (Reversed math so it doesn't face backwards)
        Vector3 directionToPlayer = (turret.position - playerTarget.position).normalized;
        directionToPlayer.y = 0; // Keep it spinning only horizontally

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            currentTurretRotation = Quaternion.Slerp(currentTurretRotation, targetRotation, rotationSpeed * Time.deltaTime);
            turret.rotation = currentTurretRotation;
        }

        // Force the mechanics to aim directly at the player, bypassing Blockbench animations entirely
        if (firePoint != null)
        {
            firePoint.LookAt(playerTarget.position);

            // force the visual water particles to match the damage laser's rotation
            if (sprayEffect != null)
            {
                sprayEffect.transform.rotation = firePoint.rotation;
            }
        }
    }

    void SprayAttack()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.enemyShoot != null)
        {
            AudioManager.Instance.playAudio(AudioManager.Instance.enemyShoot);
        }

        if (sprayEffect != null && !sprayEffect.isPlaying)
        {
            sprayEffect.Play();
        }

        if (firePoint == null) return;

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, detectionRange))
        {
            PlayerHealth pHealth = hit.collider.GetComponentInParent<PlayerHealth>();
            if (pHealth != null)
            {
                damageAccumulator += damagePerSecond * Time.deltaTime;
                if (damageAccumulator >= 1f)
                {
                    pHealth.TakeDamage(1);
                    damageAccumulator -= 1f;
                }
            }
        }
    }

    void HandleSpawning()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            if (normalEnemyPrefab != null && rearSpawnPoint != null)
            {
                Instantiate(normalEnemyPrefab, rearSpawnPoint.position, rearSpawnPoint.rotation);
            }
            spawnTimer = 0f;
        }
    }
}