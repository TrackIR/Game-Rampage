using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class HelicopterAI : MonoBehaviour
{

    public Transform playerTarget;
    public EnemyAudio audioPlayer;
    private bool playerInRange;
    private bool playerInSight;
    public float fireRange = 60f;
    public NavMeshAgent agent;
    private NavMeshPath path;
    private float elapsed = 0f; // Timer for pathfinding updates
    public float fireCooldown = 0.01f;
    private float lastFireTime = 0f;
    public Transform particleEmitter;
    public float particleDamage = 0.5f;
    private ParticleSystem ParticleSystem;
    public Transform heliBody;

    // Animation Variables
    private Animator anim;
    private int animIdleHash;
    private int animAttackHash;
    private int animForwardHash;
    private int animForwardAttackHash;
    private int animBackwardHash;
    private int animBackwardAttackHash;

    void Start()
    {
        if (playerTarget == null)
        {
            GameObject playerHeadObj = GameObject.FindGameObjectWithTag("PlayerHead");
            if (playerHeadObj != null) playerTarget = playerHeadObj.transform;
        }
        GameObject heliBodyObj = GameObject.FindGameObjectWithTag("HeliBody");
        agent = GetComponent<NavMeshAgent>();
        agent.areaMask = 1 << NavMesh.GetAreaFromName("Flyable");
        path = new NavMeshPath();
        agent.stoppingDistance = fireRange;
        if (particleEmitter != null)
        {
            ParticleSystem = particleEmitter.GetComponent<ParticleSystem>();
        }

        // Get animations
        anim = gameObject.GetComponentInChildren<Animator>();
        if (anim != null)
        {
            animIdleHash = Animator.StringToHash("Base Layer.Idle");
            animAttackHash = Animator.StringToHash("Base Layer.Attack");
            animForwardHash = Animator.StringToHash("Base Layer.Forward");
            animForwardAttackHash = Animator.StringToHash("Base Layer.ForwardAttack");
            animBackwardHash = Animator.StringToHash("Base Layer.Backward");
            animBackwardAttackHash = Animator.StringToHash("Base Layer.BackwardAttack");
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerInRange = Physics.CheckSphere(transform.position, fireRange, LayerMask.GetMask("player"));
        playerInSight = Physics.Raycast(transform.position, playerTarget.position - transform.position, out RaycastHit hit, fireRange, LayerMask.GetMask("player", "Default", "Building"));

        bool shooting = false; //Used to control animations

        if (playerInRange && playerInSight && hit.collider.CompareTag("Player"))
        {
            agent.stoppingDistance = fireRange;
            Shootplayer();
            shooting = true;
        }
        else if (playerInRange && !hit.collider.CompareTag("Player"))
        {
            agent.stoppingDistance = 0f;
            FindPlayer();
            audioPlayer.PlayForward();
        }
        else
        {
            agent.stoppingDistance = fireRange;
            FindPlayer();
        }

        UpdateAnimation(shooting);

        // update audio player for shooting
        audioPlayer.PlayWaterJet(shooting);

        // Previous Rotation code for the helicopter
        /*
        Vector3 forward = agent.velocity.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(forward);
        // Tilt down by 20 degrees while moving forward
        targetRotation *= Quaternion.Euler(agent.velocity.sqrMagnitude, 0f, 0f);
        heliBody.transform.rotation = Quaternion.Slerp(heliBody.transform.rotation, targetRotation, Time.deltaTime * 5f);
        */


    }

    void FindPlayer()
    {
        ParticleSystem.Stop();
        elapsed += Time.deltaTime;
        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;
            if (!NavMesh.SamplePosition(agent.transform.position, out NavMeshHit startHit, 2f, agent.areaMask))
            {
                Debug.LogWarning("HelicopterAI: agent is not on the NavMesh.", this);
                return;
            }

            if (!NavMesh.SamplePosition(playerTarget.position, out NavMeshHit endHit, 20f, agent.areaMask))
            {
                Debug.LogWarning("HelicopterAI: target is not on the NavMesh.", this);
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
                Debug.LogWarning($"HelicopterAI: path status {path.status}", this);
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
        if (particleEmitter == null || playerTarget == null)
        {
            return;
        }

        if (ParticleSystem == null)
        {
            return;
        }

        float speed = GetParticleSpeed(ParticleSystem);
        Vector3 launchVelocity = GetLaunchVelocity(particleEmitter.position, playerTarget.position, speed);
        if (launchVelocity.sqrMagnitude > 0.01f)
        {
            particleEmitter.rotation = Quaternion.LookRotation(launchVelocity);
        }

        ParticleSystem.Play();
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
        // If the discriminant is negative, there is no real solution, so just shoot straight at the target
        if (discriminant < 0f)
        {
            return toTarget.normalized * speed;
        }

        float sqrtDisc = Mathf.Sqrt(discriminant);
        // Use the lower angle (the higher angle would be obtained by using +sqrtDisc instead of -sqrtDisc)
        float tanTheta = (v2 - sqrtDisc) / (g * x);
        float theta = Mathf.Atan(tanTheta);
        Vector3 dirXZ = toTargetXZ.normalized;
        // Calculate the launch velocity vector
        return dirXZ * Mathf.Cos(theta) * speed + Vector3.up * Mathf.Sin(theta) * speed;
    }

    // Updates animation based on shooting state and velocity
    void UpdateAnimation(bool shooting)
    {
        if (anim == null) return;

        float speed = agent.velocity.magnitude;

        // Stationary Animations
        if (speed < 0.1f)
        {
            if (shooting)
                anim.Play(animAttackHash);
            else
                anim.Play(animIdleHash);
        }

        // Moving Animations
        else
        {
            // Get velocity of the helicopter relative to itself
            Vector3 velocity = transform.InverseTransformDirection(agent.velocity);

            // Forward
            if (velocity.z > 0)
            {
                if (shooting)
                    anim.Play(animForwardAttackHash);
                else
                    anim.Play(animForwardHash);
            }

            // Backward
            else
            {
                if (shooting)
                    anim.Play(animBackwardAttackHash);
                else
                    anim.Play(animBackwardHash);
            }
        }
    }

    void OnDestroy()
    {
        // Find all renderers on the vehicle and its children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            // Explicitly destroy the cloned materials to free up RAM
            if (r != null && r.material != null)
            {
                Destroy(r.material);
            }
        }
    }
}
