using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject Canvas;
    private Canvas UImanager;
    public bool isAlive = true;

    public GameObject deathMenu;
    public GameObject playMenu;

    private MonoBehaviour movementScript;

    private Animator anim;
    private int animDamageHash;

    void Start()
    {
        Time.timeScale = 1f;

        currentHealth = maxHealth;

        // Auto-Link UI
        if (!Canvas) Canvas = GameObject.Find("Canvas");
        if (Canvas) UImanager = Canvas.GetComponent<Canvas>();

        if (!playMenu) playMenu = GameObject.Find("PlayMenu");
        if (!deathMenu && playMenu) deathMenu = playMenu.transform.Find("DeathMenu")?.gameObject;

        anim = gameObject.GetComponentInChildren<Animator>();
        if (anim != null) animDamageHash = Animator.StringToHash("Base Layer.Damage");

        // finds one named "movement", then grabs it
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            // Check if the script name is "movement"
            if (script.GetType().Name.ToLower().Contains("movement"))
            {
                movementScript = script;
                Debug.Log("Found Movement Script: " + script.GetType().Name);
                break;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;
        if (UImanager) UImanager.GetComponent<ManageUI>().ChangeHealth(-damage);

        if (anim != null) anim.SetTrigger("Damage");

        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (UImanager) UImanager.GetComponent<ManageUI>().ChangeHealth(amount);
    }

    void Die()
    {
        if (!isAlive) return;
        isAlive = false;

        Time.timeScale = 0f;

        if (movementScript != null)
        {
            movementScript.enabled = false;
        }
        else
        {
            Debug.LogError("Could not find 'Movement' script! Cursor might still be locked.");
        }

        // Unlock Cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playMenu) playMenu.SetActive(false);
        if (deathMenu) deathMenu.SetActive(true);
    }
}