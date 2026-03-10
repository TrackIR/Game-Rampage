using UnityEngine;

[CreateAssetMenu(menuName = "Game/Game Settings")]
public class GameSettings : ScriptableObject
{
    public string difficulty = "Normal";
    public string mapIndex;
    public bool useTrackIR = true;
}