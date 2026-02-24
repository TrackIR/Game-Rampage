using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 14f;
    public Transform attackPoint;
    public LayerMask targetLayers;
    public int hitDamage = 25;

    [Header("Cooldowns")]
    public float normalAttackCooldown = 0.25f;      // seconds
    public float ultimateActivationCooldown = 5f;   // cooldown after using ultimate
    public float normalAttackTimer = 0f;
    public float ultimateCooldownTimer = 0f;

    [Header("Ultimate Settings")]
    public bool UltimateCharged = false;
    public int ultimateLength = 20; // number of FixedUpdate frames ultimate lasts
    public int UltimateThreshold = 250;
    private int lastUltimateLevel = 0;

    [Header("Ultimate Laser")]
    public GameObject ultLaserPrefab;
    public Transform ultSpawnPoint;
    public float ultLaserDuration = 3f;
    public int ultLaserDamage = 100;

    [Header("Ultimate Control")]
    public movement movement;
    public cameraMovement3D cameraMovement;
    public float slowMoScale = 0.2f;
    public float slowMoDur = 2.0f;
    public bool isInUltimate = false;


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
        if (isInUltimate)
        {
            return;
        }
            

        // reduce cooldown timers
        if (normalAttackTimer > 0f)
            normalAttackTimer -= Time.deltaTime;
        if (ultimateCooldownTimer > 0f)
            ultimateCooldownTimer -= Time.deltaTime;

        checkScore();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // activate ultimate if ready and not in cooldown
            if (UltimateCharged && ultimateCooldownTimer <= 0f && !isInUltimate)
            {
                StartCoroutine(UltimateSequence());
            }
            // Normal attack
            else if (!UltimateCharged && normalAttackTimer <= 0f)
            {
                Attack();
                normalAttackTimer = normalAttackCooldown;
            }
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
        if (ultLaserPrefab != null && ultSpawnPoint != null)
        {
            GameObject ultObj = Instantiate(
                ultLaserPrefab,
                ultSpawnPoint.position,
                ultSpawnPoint.rotation   // copy player/spawn rotation
            );

            // Parent it so one end stays at spawn point
            ultObj.transform.SetParent(ultSpawnPoint);

            UltimateLaser ultScript = ultObj.GetComponent<UltimateLaser>();
            if (ultScript != null)
            {
                ultScript.damagePerSecond = ultLaserDamage;
                ultScript.duration = ultLaserDuration;
                ultScript.targetLayers = targetLayers;
            }
        }
    }

    private IEnumerator UltimateSequence()
    {
        isInUltimate = true;

        // slow motion start
        Time.timeScale = slowMoScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // switch to 1st person
        if (cameraMovement != null)
            cameraMovement.is3rdPerson = false;

        yield return new WaitForSecondsRealtime(slowMoDur);

        // restore time
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // disable player movement & rotation
        if (movement != null)
            movement.enabled = false;

        // spawn laser
        UltAttack();

        yield return new WaitForSeconds(ultLaserDuration);

        EndUltimate();
    }

    private void EndUltimate()
    {
        // re-enable movement
        if (movement != null)
            movement.enabled = true;

        // return to 3rd person
        if (cameraMovement != null)
            cameraMovement.is3rdPerson = true;

        UltimateCharged = false;
        ultimateCooldownTimer = ultimateActivationCooldown;

        isInUltimate = false;
    }



    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
