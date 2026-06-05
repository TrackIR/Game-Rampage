using UnityEngine;
using UnityEngine.UI;

public class ApplyLogoImage : MonoBehaviour
{

    public Sprite[] Sprites;

    // Choose a random sprite from the list of logo sprites and apply it to an image.
    void Start()
    {

        if (Sprites == null || Sprites.Length == 0)
        {
            return;
        }



        int index = Random.Range(0, Sprites.Length);
        GetComponent<Image>().sprite = Sprites[index];


    }
}
