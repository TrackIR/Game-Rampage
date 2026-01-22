using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 14f;
    public Transform attackPoint;

    // Using a LayerMask allows for multiple layers (Buildings AND Enemies)
    public LayerMask targetLayers;

    public int hitDamage = 25; // How much damage a hit does to an enemy

    private Animator anim;
    private int animPunchHash;

    void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();

        if (anim != null)
        {
            animPunchHash = Animator.StringToHash("Base Layer.Punch");
        }

    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    void Attack()
    {
        Collider[] hitObjects = Physics.OverlapSphere(attackPoint.position, attackRange, targetLayers);

        // create a temporary list to track unique enemies hit in this swing
        List<EnemyHealth> enemiesHit = new List<EnemyHealth>();

        foreach (Collider hit in hitObjects)
        {
            // building destruction
            BuildingDestruction building = hit.GetComponent<BuildingDestruction>();
            if (building != null)
            {
                building.TakeDamage();
            }

            // enemy attack
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                // Have the player already hit this specific enemy instance
                if (!enemiesHit.Contains(enemy))
                {
                    // If not, damage them
                    enemy.TakeDamage(hitDamage);

                    // Add them to the list so it doesn't hit them again this frame
                    enemiesHit.Add(enemy);
                }
            }
        }

        anim.SetTrigger("Punch");
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}