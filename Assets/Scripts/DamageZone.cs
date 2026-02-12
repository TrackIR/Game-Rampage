using UnityEngine;

public class DamageZone : MonoBehaviour
{

    public float dps = 10.0f;

    private bool playerContact = false;
    private PlayerHealth playerHealth;
    private float damageCount;

    // Update is called once per frame
    void Update()
    {
        if (playerContact && playerHealth != null)
        {
            damageCount += dps * Time.deltaTime;
            int damage = Mathf.FloorToInt(damageCount); //Turn the damage into an int so it can be used with takeDamage
            playerHealth.TakeDamage(damage);
            damageCount -= damage;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure the player enters the collider
        {
            playerContact = true;
            playerHealth = other.GetComponent<PlayerHealth>();
            damageCount = 0f;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerContact = false;
            playerHealth = null;
            damageCount = 0f; // Set damage counter back to zero
        }

    }
}
