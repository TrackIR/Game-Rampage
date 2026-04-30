using UnityEngine;
using UnityEngine.AI;

public class FiretruckAI : MonoBehaviour
{
    [Header("References")]
    public Transform playerTarget;
    private Transform playerHead;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Turret & Spray Settings")]
    public Transform turret;
    public ParticleSystem sprayEffect;
    public Transform firePoint;
    public float detectionRange = 15f;
    public float rotationSpeed = 5f;
    public float damagePerSecond = 0.5f;
    public float particleDamage = 0.5f;
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
            if (playerObj != null) {
                playerTarget = playerObj.transform;
                playerHead = GameObject.FindGameObjectWithTag("PlayerHead").transform;
            }
        }
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (agent != null)
        {
            agent.speed = speed;
            agent.updateRotation = false;
        }

        path = new NavMeshPath();

        if (sprayEffect != null)
        {
            sprayEffect.Stop();

            if (firePoint != null)
            {
                sprayEffect.transform.SetParent(firePoint, true);
            }
        }

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
            AimTurret();
            SprayAttack();
        }
        else
        {
            if (animator != null) animator.SetBool("IsDriving", true);
            findPlayer();
            HandleDrivingRotation();
        }
    }

    void LateUpdate()
    {
        if (turret == null || playerTarget == null) return;

        // Spin the visual Turret (Reversed math so it doesn't face backwards)
        //Vector3 directionToPlayer = (turret.position - playerTarget.position).normalized;
        //directionToPlayer.y = 0;

        //if (directionToPlayer != Vector3.zero)
        {
            //Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            //currentTurretRotation = Quaternion.Slerp(currentTurretRotation, targetRotation, rotationSpeed * Time.deltaTime);
            //turret.rotation = currentTurretRotation;
            AimTurret();
        }


        // Force the invisible damage laser to point at the player
        if (firePoint != null)
        {
            //firePoint.LookAt(playerTarget.position);

            // Physically glue the rescued WaterSpray back to the barrel's location, 
            // and force it to stare exactly at the player
            //if (sprayEffect != null)
            //{
            //    sprayEffect.transform.position = firePoint.position;
            //    sprayEffect.transform.LookAt(playerTarget.position);
            //}
        }
    }

    void HandleDrivingRotation()
    {
        if (agent != null && agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 moveDirection = agent.velocity.normalized;
            Vector3 flippedDirection = -moveDirection;
            flippedDirection.y = 0;

            if (flippedDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(flippedDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
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

    }

    void AimTurret()
    {
        if (firePoint == null || turret == null || playerHead == null || sprayEffect == null)
        {
            return;
        }

        float speed = GetParticleSpeed(sprayEffect);
        Vector3 launchVelocity = GetLaunchVelocity(firePoint.position, playerHead.position, speed);
        if (launchVelocity.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(-launchVelocity);
        firePoint.rotation = Quaternion.Slerp(firePoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        currentTurretRotation = firePoint.rotation; 
        turret.rotation = currentTurretRotation;
    }

    private float GetParticleSpeed(ParticleSystem particleSystem)
    {
        ParticleSystem.MainModule main = particleSystem.main;
        ParticleSystem.MinMaxCurve curve = main.startSpeed;
        return curve.constant; // particle speed is constant
    }

    private Vector3 GetLaunchVelocity(Vector3 start, Vector3 target, float speed)
    {
        if (speed <= 0.01f)
        {
            return Vector3.zero;
        }

        Vector3 toTarget = target - start;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        float x = toTargetXZ.magnitude;
        float y = toTarget.y;

        if (x < 0.01f)
        {
            return toTarget.normalized * speed;
        }
        // Calculate the launch angle using the physics formula for projectile motion
        float g = -Physics.gravity.y;
        float v2 = speed * speed;
        float v4 = v2 * v2;
        float discriminant = v4 - g * (g * x * x + 2f * y * v2);
        // If the discriminant is negative shoot straight at the target
        if (discriminant < 0f)
        {
            return toTarget.normalized * speed;
        }

        float sqrtDisc = Mathf.Sqrt(discriminant);
        // Use the lower angle (the higher angle use +sqrtDisc instead of -sqrtDisc)
        float tanTheta = (v2 - sqrtDisc) / (g * x);
        float theta = Mathf.Atan(tanTheta);
        Vector3 dirXZ = toTargetXZ.normalized;
        // Calculate the launch velocity vector
        return dirXZ * Mathf.Cos(theta) * speed + Vector3.up * Mathf.Sin(theta) * speed;
    }

    void OnParticleCollision(GameObject other)
    {
        if (other == null || !other.CompareTag("Player"))
        {
            return;
        }

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(particleDamage);
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