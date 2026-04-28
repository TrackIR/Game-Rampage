using UnityEngine;

public class tutorialZone : MonoBehaviour
{
    public  GameObject TutorialTextParent;

    void Start()
    {
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialTextParent.SetActive(false);
        }
    }
}
