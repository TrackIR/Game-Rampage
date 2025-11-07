using UnityEngine;

public class EnemyAI : MonoBehaviour
{
     // Basic Settings
     public Transform playerTarget;
     public float detectionRange = 50f;
     public float fireRate = 1f; // Shots per second

     // Projectile Info
     public GameObject projectilePrefab;
     public Transform firePoint;

     // Private
     private float fireCooldownTimer = 0f;
     //
     void Update()
     {
          // 1. Always count down the cooldown timer
          if (fireCooldownTimer > 0)
          {
               fireCooldownTimer -= Time.deltaTime;
          }

          // 2. If no target, do nothing
          if (playerTarget == null)
          {
               return;
          }

          // 3. Check if target is in range
          float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

          if (distanceToPlayer <= detectionRange)
          {
               // If in range, perform actions

               // TODO: Add  aiming logic here (basic implementation done)
               transform.LookAt(playerTarget);

               // If cooldown is ready, shoot
               if (fireCooldownTimer <= 0)
               {
                    Shoot();
                    fireCooldownTimer = 1f / fireRate; // Reset cooldown
               }
          }
     }

     void Shoot()
     {
          // TODO: Create the bullet at the firePoint's position
          if (projectilePrefab != null && firePoint != null)
          {
               Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
          }
     }
}