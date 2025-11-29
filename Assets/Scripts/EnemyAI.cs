using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Basic Settings
    public Transform playerTarget;
    public float detectionRange = 50f; // How far away the enemy detects the player
    public float fireRate = 1f; // Shots per second
    public float rotationSpeed = 5f; // How fast the enemy turns
    public float speed = 5f; // How fast the enemy moves

    // Projectile Info
    public GameObject projectilePrefab;
    public Transform firePoint;

    private float fireCooldownTimer = 0f;

    void Update()
    {
        // Always count down the cooldown timer
        if (fireCooldownTimer > 0)
        {
            fireCooldownTimer -= Time.deltaTime;
        }

        // If no target, do nothing
        if (playerTarget == null)
        {
            return;
        }

        // Check if target is in range
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= detectionRange)
        {
            // If in range, aim at player

            AimAtPlayer();

            // If cooldown is ready, shoot
            if (fireCooldownTimer <= 0)
            {
                Shoot();
                fireCooldownTimer = 1f / fireRate; // Reset cooldown
            }
        }
        if (distanceToPlayer > detectionRange)
        {
            // If not in range, move towards player
            transform.position += transform.forward * speed * Time.deltaTime;

        }
    }

    void AimAtPlayer()
    {
        // Get the direction from the enemy to the player
        Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;

        // Create the rotation needed to look at the player
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        // Smoothly rotate towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

    }

    void Shoot()
    {
        // Create the bullet at firePoint's position
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }
}