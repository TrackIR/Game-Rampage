using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Basic Settings
    public Transform playerTarget;
    public float detectionRange = 15f; // range for spray attack
    public float rotationSpeed = 5f;
    public float speed = 5f;
    public float gravity = 5f; // Constant downward force

    // Spray Settings
    public int damagePerSecond = 5;    // damage every second
    public ParticleSystem sprayEffect;
    public Transform firePoint;        // Where the spray comes from

    private float damageAccumulator = 0f; // Stores partial damage

    void Start()
    {
        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTarget = playerObj.transform;
        }

        // Ensure spray is off at the start
        if (sprayEffect != null) sprayEffect.Stop();
    }

    void Update()
    {
        // Apply constant downward force
        transform.position += Vector3.down * gravity * Time.deltaTime;

        if (playerTarget == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        // If close, spray. If far, chase player.
        if (distanceToPlayer <= detectionRange)
        {
            AimAtPlayer();
            SprayAttack(); // Fire the attack
        }
        else
        {
            // Stop spraying if player runs away
            if (sprayEffect != null && sprayEffect.isPlaying)
            {
                sprayEffect.Stop();
            }

            AimAtPlayer();
            // Move forward
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    void AimAtPlayer()
    {
        Vector3 directionToPlayer = (playerTarget.position - transform.position);

        // Flatten the vertical direction so they don't look up too high
        directionToPlayer.y *= 0.4f;

        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void SprayAttack()
    {
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