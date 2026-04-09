using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using System;

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
    public GameSettings gameSettings;
    public CameraShake cameraShake;
    public float cameraShakeMag = 0.2f;
    private Animator anim;
    private int animPunchHash;
    private ManageUI uiManager;

    [Header("Runtime State")]
    public float normalAttackTimer = 0f;
    public float ultimateCooldownTimer = 0f;
    private bool isInUltimate = false;
    private int lastUltimateLevel = 0;

    // INPUT SYSTEM
    private PlayerInput input;
    private InputAction attackAction;

    void Awake()
    {
        input = new PlayerInput();

        if (gameSettings.useTrackIR)
        {
            attackAction = input.TrackIR.Attack;
        }
        else
        {
            attackAction = input.KeyboardMouse.Attack;
        }
    }

    void OnEnable()
    {
        input.Enable();

        if (gameSettings.useTrackIR)
        {
            input.TrackIR.Enable();
        }
        else
        {
            input.KeyboardMouse.Enable();
        }

        attackAction.performed += OnAttack;
    }

    void OnDisable()
    {
        attackAction.performed -= OnAttack;

        if (gameSettings.useTrackIR)
        {
            input.TrackIR.Disable();
        }
        else
        {
            input.KeyboardMouse.Disable();
        }

        input.Disable();
    }

    void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        uiManager = FindFirstObjectByType<ManageUI>();

        if (anim != null)
            animPunchHash = Animator.StringToHash("Base Layer.Punch");
    }

    void Update()
    {
        if (isInUltimate)
        {
            AimLaserAtCursor();
            return;
        }

        if (normalAttackTimer > 0f)
            normalAttackTimer -= Time.deltaTime;

        if (ultimateCooldownTimer > 0f)
            ultimateCooldownTimer -= Time.deltaTime;

        checkScore();
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        if (isInUltimate)
            return;

        if (ultimateCharged && ultimateCooldownTimer <= 0f)
        {
            StartCoroutine(UltimateSequence());
        }
        else if (!ultimateCharged && normalAttackTimer <= 0f && ultimateCooldownTimer <= 0f)
        {
            Attack();
            normalAttackTimer = normalAttackCooldown;
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
                building.TakeDamage();

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
            Debug.Log(hit.distance + " hit: " + hit.collider.name);
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 1000f;
        }

        Vector3 desiredDir = (targetPoint - ultSpawnPoint.position).normalized;
        Vector3 currentDir = ultSpawnPoint.forward;

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

            ultObj.transform.SetParent(ultSpawnPoint);

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

        if (cameraMovement != null)
            cameraMovement.transitionSpeed = 3;

        cameraMovement.is3rdPerson = false;

        while (cameraMovement.cameraBlend > 0.01f)
            yield return null;

        playerHead.SetActive(false);

        if (movement != null)
            movement.enabled = false;

        UltAttack();

        // shake it shake it baby
        StartCoroutine(cameraShake.Shake(ultLaserDuration - 0.5f, cameraShakeMag));

        yield return new WaitForSeconds(ultLaserDuration);

        yield return StartCoroutine(EndUltimate());
    }

    // Changed from void to IEnumerator
    private IEnumerator EndUltimate()
    {
        playerHead.SetActive(true);

        if (cameraMovement != null)
            cameraMovement.transitionSpeed = 3f;

        cameraMovement.is3rdPerson = true;

        while (cameraMovement.cameraBlend < 0.99f)
            yield return null;

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