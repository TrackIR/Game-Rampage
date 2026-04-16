using UnityEngine;

public class tutorialZone : MonoBehaviour
{
    public  GameObject TutorialText;

    void Start()
    {
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialText.SetActive(false);
        }
    }
}
