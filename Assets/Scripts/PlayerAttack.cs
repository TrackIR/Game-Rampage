using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{

    public float attackRange = 14f;
    public Transform attackPoint;

    // Using a LayerMask allows for multiple layers (Buildings AND Enemies)
    public LayerMask targetLayers;

    public int hitDamage = 25; // How much damage a hit does to an enemy

    ManageUI uiManager;
    public bool UltimateCharged = false;
    private bool isUlt = false;
    public int ultimateLength = 20;
    private int ultCount = 0; // counter for physic frames
    public int UltimateThreshold = 250; // how many points until the player gets an ultimate
    private int lastUltimateLevel = 0;

    private Animator anim;
    private int animPunchHash;

    void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        uiManager = FindFirstObjectByType<ManageUI>();

        if (anim != null)
        {
            animPunchHash = Animator.StringToHash("Base Layer.Punch");
        }

    }

    void Update()
    {
        checkScore();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (UltimateCharged)
            {
                UltAttack();
                UltimateCharged = false;
            }
            else
            {
                Attack();
            }
        }
    }

    void FixedUpdate()
    {
        if (isUlt && (ultCount < ultimateLength))
        {
            Attack();
            ultCount++;
            print($"Count: {ultCount}");
        }
        else if (ultCount == ultimateLength)
        {
            isUlt = false;
            ultCount = 0;
        }
    }

    void checkScore()
    {
        if (uiManager == null) return;

        int score = uiManager.score;

        int currentLevel = score / UltimateThreshold;

        // only sets UltimateCharged to true every xUltimateThreshold
        if (currentLevel > lastUltimateLevel)
        {
            UltimateCharged = true;
            Debug.Log("Ult good");
            lastUltimateLevel = currentLevel;
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

    void UltAttack()
    {
        Debug.Log("Ult");
        isUlt = true;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}