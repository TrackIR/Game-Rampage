using UnityEngine;

public class BuildingDestruction : MonoBehaviour
{
    public int health = 3; // How many hits it takes to destroy a building

    public void TakeDamage()
    {
        health--; // Subtract 1 from health

        Debug.Log("Building hit! Remaining Health: " + health);

        // Flash red so you know building was hit
        GetComponent<Renderer>().material.color = Color.red;
        Invoke("ResetColor", 0.5f); // Reset color after 0.5 seconds

        // Check if destroyed
        if (health <= 0)
        {
            Collapse();
            Debug.Log("Destroyed a building");
        }
    }

    void ResetColor()
    {
        // Return to original color
        GetComponent<Renderer>().material.color = Color.gray;
    }

    void Collapse()
    {
        // Destroy the building game object
        Destroy(gameObject);
    }
}