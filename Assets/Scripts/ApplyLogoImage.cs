using UnityEngine;

public class ApplyLogoImage : MonoBehaviour
{

    public Material[] materials;

    // Choose a random material from the list of logo materials and apply it to a plane.
    void Start()
    {

        if (materials == null || materials.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, materials.Length);
        GetComponent<Renderer>().material = materials[index];
    }
}
