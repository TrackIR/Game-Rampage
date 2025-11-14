using UnityEngine;
using TMPro; //Using this to update text

public class ManageScore : MonoBehaviour
{


    public TMP_Text scoreObject;
    private int score = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeScore(0);
        ChangeScore(100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void ChangeScore(int amount)
    {
        score += amount;


        string scoreString = "Score: " + score;

        scoreObject.text = scoreString;
    }
}
