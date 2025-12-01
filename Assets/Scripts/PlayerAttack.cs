using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 5f; // How far you can reach
    public Transform attackPoint;  // An empty object where the attack happens
    public LayerMask buildingLayer; // To make sure only buildings are hit

    void Update()
    {
        // Check for Left Mouse Button
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    void Attack()
    {

        // Detect buildings in range (damaging enemies is next goal)
        // creates a sphere at the attackPoint and gathers everything it touches
        Collider[] hitObjects = Physics.OverlapSphere(attackPoint.position, attackRange, buildingLayer);

        // Damage them
        foreach (Collider hit in hitObjects)
        {
            // Check if the object hit has the Building script
            BuildingDestruction building = hit.GetComponent<BuildingDestruction>();
            if (building != null)
            {
                building.TakeDamage();
                Debug.Log("Hit a building");
            }
        }
    }
}