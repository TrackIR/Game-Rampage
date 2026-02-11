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

    public NavMeshAgent agent;


    private const float gravity = -9.81f;
    private NavMeshPath path;

    // Spray Settings
    public int damagePerSecond = 5;    // damage every second
    public ParticleSystem sprayEffect;
    public Transform firePoint;        // Where the spray comes from

    private Vector3 gravityVector = new Vector3(0, gravity, 0);
    private float damageAccumulator = 0f; // Stores partial damage

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


        // Ensure spray is off at the start
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

            path = new NavMeshPath();

            agent.destination = playerTarget.position;

            bool canSetPath = NavMesh.CalculatePath(
                agent.transform.position, 
                agent.destination,
                NavMesh.AllAreas,
                path);

            if (canSetPath){
                agent.SetPath(path);
            }

                
        }
        if (sprayEffect.isPlaying)
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