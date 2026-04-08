using UnityEngine;

public class StompDamage : MonoBehaviour
{
    [Range(1f, 100f)] public int damage;
    [Range(0.1f, 5.0f)] public int size;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(size, size, size); // Set the size of the collider
    }

    private void OnTriggerEnter(Collider other)
    {
        // Make sure object that collides is an ememy
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}
