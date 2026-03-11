using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 14f;
    public int hitDamage = 25;
    public Transform attackPoint;
    public LayerMask targetLayers;

    [Header("Cooldowns")]
    public float normalAttackCooldown = 0.25f;
    public float ultimateActivationCooldown = 3f;

    [Header("Ultimate Settings")]
    public bool ultimateCharged = false;
    public int ultimateThreshold = 250;
    public int ultimateLength = 20;
    public float ultimateSlowmoSpeed = 0.25f;

    [Range(0f, 1f)]
    public float beamWeight = 0.01f;

    public GameObject ultLaserPrefab;
    public Transform ultSpawnPoint;
    public float ultLaserDuration = 7f;
    public int ultLaserDamage = 10;

    [Header("Movement / Player References")]
    public movement movement;
    public cameraMovement3D cameraMovement;
    public GameObject playerHead;

    [Header("References / Animation")]
    public GameObject cursor;
    private Animator anim;
    private int animPunchHash;
    private ManageUI uiManager;

    [Header("Runtime State")]
    public float normalAttackTimer = 0f;
    public float ultimateCooldownTimer = 0f;
    private bool isInUltimate = false;
    private int lastUltimateLevel = 0;

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
            AimLaserAtCursor();
            return;
        }

        // reduce cooldown timers
        if (normalAttackTimer > 0f)
        {
            normalAttackTimer -= Time.deltaTime;
        }
        if (ultimateCooldownTimer > 0f)
        {
            ultimateCooldownTimer -= Time.deltaTime;
        }

        checkScore();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // activate ultimate if ready and not in cooldown
            if (ultimateCharged && ultimateCooldownTimer <= 0f && !isInUltimate)
            {
                StartCoroutine(UltimateSequence());
            }
            // Normal attack
            else if (!ultimateCharged && normalAttackTimer <= 0f && (ultimateCooldownTimer <= 0f))
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
        int currentLevel = score / ultimateThreshold;

        if (currentLevel > lastUltimateLevel)
        {
            ultimateCharged = true;
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

    void AimLaserAtCursor()
    {
        if (cursor == null) return;

        Camera cam = Camera.main;

        Vector3 screenPos = cursor.transform.position;
        Ray ray = cam.ScreenPointToRay(screenPos);

        RaycastHit hit;
        Vector3 targetPoint;
        int layerMask = ~LayerMask.GetMask("player");

        if (Physics.Raycast(ray, out hit, 1000f, layerMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 1000f;
        }

        // desired direction toward cursor
        Vector3 desiredDir = (targetPoint - ultSpawnPoint.position).normalized;

        // current beam direction
        Vector3 currentDir = ultSpawnPoint.forward;

        // smooth the direction
        Vector3 smoothedDir = Vector3.Lerp(currentDir, desiredDir, beamWeight).normalized;

        ultSpawnPoint.rotation = Quaternion.LookRotation(smoothedDir);
    }

    void UltAttack()
    {
        if (ultLaserPrefab != null && ultSpawnPoint != null)
        {
            GameObject ultObj = Instantiate(
                ultLaserPrefab,
                ultSpawnPoint.position,
                ultSpawnPoint.rotation
            );

            // parent it so one end stays at spawn point
            ultObj.transform.SetParent(ultSpawnPoint);

            // laser movement
            //AimLaserAtCursor();

            // change laser variables
            UltimateLaser ultScript = ultObj.GetComponent<UltimateLaser>();
            if (ultScript != null)
            {
                ultScript.damagePerSecond = ultLaserDamage;
                ultScript.targetLayers = targetLayers;
            }
        }
    }

    private IEnumerator UltimateSequence()
    {
        isInUltimate = true;

        // switch to first person
        if (cameraMovement != null)
            cameraMovement.transitionSpeed = 3;
        cameraMovement.is3rdPerson = false;

        while (cameraMovement.cameraBlend > 0.01f)
        {
            Debug.Log("Waiting for 1st person transition");
            yield return null;
        }

        // hide player head
        playerHead.SetActive(false);

        // disable player movement & rotation
        if (movement != null)
            movement.enabled = false;

        // slow down time
        //Time.timeScale = ultimateSlowmoSpeed;

        // spawn laser
        UltAttack();

        yield return new WaitForSeconds(ultLaserDuration);

        // set time to normal
        //Time.timeScale = 1f;

        yield return StartCoroutine(EndUltimate());
    }

    private IEnumerator EndUltimate()
    {
        // show player head
        playerHead.SetActive(true);

        // return to third person
        if (cameraMovement != null)
            cameraMovement.transitionSpeed = 3f;
        cameraMovement.is3rdPerson = true;

        while (cameraMovement.cameraBlend < 0.99f)
        {
            Debug.Log("Waiting for 3rd person transition");
            yield return null;
        }

        // re-enable movement
        if (movement != null)
            movement.enabled = true;

        ultimateCharged = false;
        ultimateCooldownTimer = ultimateActivationCooldown;

        isInUltimate = false;
    }



    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
