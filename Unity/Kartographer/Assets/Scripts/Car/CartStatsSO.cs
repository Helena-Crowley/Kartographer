using UnityEngine;

[CreateAssetMenu(fileName = "CartStatsSO", menuName = "Scriptable Objects/CartStatsSO")]
public class CartStatsSO : ScriptableObject
{
    public int health = 100;
    [Tooltip("Battery life in percentage (10 means 100% battery life)")]
    public int batteryLife = 10; //is percentage (i.e., 10 means 10% battery life)
}
