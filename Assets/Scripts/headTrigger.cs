using UnityEngine;

public class headTrigger : MonoBehaviour
{
    public GameObject player;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHead"))
        {
            GameObject head = other.gameObject;
            head.SetActive(false);
            gameObject.SetActive(false);
            player.SetActive(true);
            GameManager.Instance.StartGamePhase();
        }
    }
}
