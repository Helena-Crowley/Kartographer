using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBaseStats", menuName = "Scriptable Objects/PlayerBaseStats")]
public class PlayerBaseStatsSO : ScriptableObject
{
    public int health = 100;
    public int stamina = 100;
    public float moveSpeed = 5f;
}
