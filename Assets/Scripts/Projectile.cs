using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    // Basic Settings
    public float speed = 75f;   // speed of projectile
    public float lifeTime = 5f; // How long it lives before destroying itself
    public float damage = 0.5f;     // How much damage to deal
    [Header("Ballistic Aim")]
    public bool useBallisticAim = true;
    public Transform target;
    private bool hasHit = false; // flag ensures the projectile only deals damage once
    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }
        // Make it move forward
        if (rb != null)
        {
            rb.linearVelocity = GetLaunchVelocity();
        }
        // Set it to destroy itself after 'lifeTime' seconds
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        //increase projectile size over time for visual effect
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 100.0f, Time.deltaTime * 0.02f);
        // Rotate the projectile to face the direction it's moving
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            Vector3 backward = -rb.linearVelocity.normalized;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, backward);
        }
    }

    private Vector3 GetLaunchVelocity()
    {
        if (!useBallisticAim || target == null)
        {
            return transform.forward * speed;
        }
        Vector3 toTarget = target.position - transform.position;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        // Distance to target from current position
        float x = toTargetXZ.magnitude;
        // Height difference between target and current position
        float y = toTarget.y;

        if (x < 0.01f)
        {
            return transform.forward * speed;
        }
        float g = -Physics.gravity.y;
        // Calculate the launch angle using the physics formula for projectile motion
        float v2 = speed * speed;
        float v4 = v2 * v2;
        float discriminant = v4 - g * (g * x * x + 2f * y * v2);
        // If the discriminant is negative, there is no real solution, so just shoot straight at the target
        if (discriminant < 0f)
        {
            return toTarget.normalized * speed;
        }
        // Use the lower angle (the higher angle would be obtained by using +sqrtDisc instead of -sqrtDisc)
        float sqrtDisc = Mathf.Sqrt(discriminant);
        float tanTheta = (v2 - sqrtDisc) / (g * x);
        float theta = Mathf.Atan(tanTheta);
        Vector3 dirXZ = toTargetXZ.normalized;
        // Calculate the launch velocity vector
        return dirXZ * Mathf.Cos(theta) * speed + Vector3.up * Mathf.Sin(theta) * speed;
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