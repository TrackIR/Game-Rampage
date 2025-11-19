using UnityEngine;
using TMPro; //Using this to update text

// When using this, remember to adjust the pivot of the health bar fil to 0, 0.5, or it will reduce from both ends

public class ManageHealth : MonoBehaviour
{

    public TMP_Text healthObject;
    public RectTransform healthBarObject, healthBarObjectFill;
    public AudioSource trashIn, trashOut;
    public int maxHealth;
    private int health = 0;



    void Start()
    {
        ChangeHealth(maxHealth); //Set health to the maximum

        ChangeHealth(-20); //Set health to the maximum

    }

    void ChangeHealth(int amount)
    {
        health += amount;

        if (health <= 0)
        {
            print("HEALTH AT 0. ACT ON IT HERE");
        }
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        if (amount > 0) // Play the sound for gaining health
        {
            trashIn.Play(0);
        }
        else if (amount < 0) // Play the sound for losing health
        {
            trashOut.Play(0);
        }


        string healthString = "Health: " + health + "/" + maxHealth;

        healthObject.text = healthString;



        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {

        float healthPercent = (float)health / (float)maxHealth;

        Vector2 size = healthBarObjectFill.sizeDelta;
        size.y = healthBarObject.sizeDelta.y * healthPercent;
        healthBarObjectFill.sizeDelta = size;
    }

}


