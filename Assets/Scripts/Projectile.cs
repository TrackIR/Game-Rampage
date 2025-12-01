using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    // Basic Settings
    public float speed = 75f;   // speed of projectile
    public float lifeTime = 5f; // How long it lives before destroying itself
    public int damage = 5;     // How much damage to deal


    private bool hasHit = false; // flag ensures the projectile only deals damage once

    void Start()
    {
        // Get the Rigidbody component
        Rigidbody rb = GetComponent<Rigidbody>();

        // Make it move forward
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        // Set it to destroy itself after 'lifeTime' seconds
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {

        if (hasHit)
        {
            return;
        }
        // Set to true, so this only ever runs once per projectile
        hasHit = true;

        // Try to find a PlayerHealth script on the object hit
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        // If script was found, call the TakeDamage function
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        // Destroy the projectile on impact with anything (building or player)
        Destroy(gameObject);
    }
}