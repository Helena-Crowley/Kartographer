using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class WheelSlot : NetworkBehaviour
{
    public Transform mountPoint;
    public NetworkVariable<bool> isOccupied;

    void Awake()
    {
        isOccupied.Value = false;
    } 

    public void CheckOccupiedStatus()
    {
        if (GetComponentInChildren<Tire>() != null)
        {
            isOccupied.Value = true;
            Debug.Log("Played attach sound");
        }
        else
        {
            isOccupied.Value = false;
        }
    }
}