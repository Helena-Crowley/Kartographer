using UnityEngine;

[CreateAssetMenu(fileName = "SpawnData", menuName = "Game/Spawn Data")]
public class SpawnData : ScriptableObject
{
    public string sceneName;                // Scene this data belongs to
    public Vector3[] positions;        // Player spawn positions
    public Quaternion[] rotations;     // Player spawn rotations
}
