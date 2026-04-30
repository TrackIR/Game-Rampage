using UnityEditor;
using UnityEngine;

public class TestMem : MonoBehaviour
{
    void Start()
    {
        EditorUtility.UnloadUnusedAssetsImmediate();
        System.GC.Collect();
    }

}
