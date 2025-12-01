using UnityEngine;

public class ApplyMaterial : MonoBehaviour
{
    // Should be used on an empty object, with the building as children.



    public Material[] materialList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ApplyBuildingMaterial();
    }

    void ApplyBuildingMaterial()
    {
        // Get material from the defined list,
        // currently using black, white, and lit for buildings
        Material material = materialList[Random.Range(0, materialList.Length)];

        Renderer[] renderers = GetComponentsInChildren<Renderer>();

       
        foreach (Renderer r in renderers)
        {
            r.material = material;  // Apply the material to the object's children
        }
    }
}
