using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;


public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject Canvas;
    private Canvas UImanager;
    public bool isAlive = true;

    public SceneAsset gameScene;

    private Animator anim;
    private int animDamageHash;

    void Start()
    {
        // Set health at the start
        currentHealth = maxHealth;
        UImanager = Canvas.GetComponent<Canvas>();

        anim = gameObject.GetComponentInChildren<Animator>();

        if (anim != null)
        {
            animDamageHash = Animator.StringToHash("Base Layer.Damage");
        }
    }

    // function that other scripts can call
    public void TakeDamage(int damage)
    {
        // Reduce health
        currentHealth -= damage;
        UImanager.GetComponent<ManageUI>().ChangeHealth(-damage);
        Debug.Log(gameObject.name + " health: " + currentHealth);

        // Play damage animation
        anim.SetTrigger("Damage");

        // Check if the player is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        isAlive = false;
        string sceneName = gameScene.name;
        SceneManager.LoadScene(sceneName);
    }
}