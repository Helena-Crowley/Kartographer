using UnityEngine;

[CreateAssetMenu(fileName = "SpawnData", menuName = "Game/Spawn Data")]
public class SpawnData : ScriptableObject
{
    public string sceneName;                // Scene this data belongs to
    public Vector3[] spawnPositions;        // Player spawn positions
    public Quaternion[] spawnRotations;     // Player spawn rotations
}
