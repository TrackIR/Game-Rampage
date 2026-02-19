using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 14f;
    public Transform attackPoint;
    public LayerMask targetLayers;
    public int hitDamage = 25;

    [Header("Cooldowns")]
    public float normalAttackCooldown = 0.5f;      // seconds
    public float ultimateAttackCooldown = 1f;       // seconds between ult hits
    private float normalAttackTimer = 0f;
    private float ultimateAttackTimer = 0f;

    [Header("Ultimate Settings")]
    public bool UltimateCharged = false;
    public int ultimateLength = 20;
    public int UltimateThreshold = 250;

    private bool isUlt = false;
    private int ultCount = 0;
    private int lastUltimateLevel = 0;

    [Header("References / Animation")]
    private Animator anim;
    private int animPunchHash;
    private ManageUI uiManager;

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
        // Reduce cooldown timers
        if (normalAttackTimer > 0f)
            normalAttackTimer -= Time.deltaTime;
        if (ultimateAttackTimer > 0f)
            ultimateAttackTimer -= Time.deltaTime;

        checkScore();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (UltimateCharged)
            {
                if (ultimateAttackTimer <= 0f) // Only attack if ultimate cooldown is over
                {
                    UltAttack();
                    UltimateCharged = false;
                    ultimateAttackTimer = ultimateAttackCooldown;
                }
            }
            else
            {
                if (normalAttackTimer <= 0f) // Only attack if normal cooldown is over
                {
                    Attack();
                    normalAttackTimer = normalAttackCooldown;
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Automatic ultimate attack while ult is active
        if (isUlt && ultCount < ultimateLength)
        {
            if (ultimateAttackTimer <= 0f)
            {
                Attack();
                ultCount++;
                ultimateAttackTimer = ultimateAttackCooldown;
            }
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

        if (currentLevel > lastUltimateLevel)
        {
            UltimateCharged = true;
            Debug.Log("Ult ready");
            lastUltimateLevel = currentLevel;
        }
    }

    void Attack()
    {
        Collider[] hitObjects = Physics.OverlapSphere(attackPoint.position, attackRange, targetLayers);

        List<EnemyHealth> enemiesHit = new List<EnemyHealth>();

        foreach (Collider hit in hitObjects)
        {
            BuildingDestruction building = hit.GetComponent<BuildingDestruction>();
            if (building != null)
            {
                building.TakeDamage();
            }

            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

            if (enemy != null && !enemiesHit.Contains(enemy))
            {
                enemy.TakeDamage(hitDamage);
                enemiesHit.Add(enemy);
            }
        }

        anim.SetTrigger("Punch");
    }

    void UltAttack()
    {
        Debug.Log("Ultimate Attack");
        isUlt = true;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
