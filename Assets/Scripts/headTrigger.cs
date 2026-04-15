using UnityEditor.UI;
using UnityEngine;

public class headTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject player;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject head = other.gameObject;
            head.SetActive(false);
            gameObject.SetActive(false);
            player.SetActive(true);
        }
    }
}
