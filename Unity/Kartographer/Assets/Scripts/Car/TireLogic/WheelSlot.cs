using UnityEngine;

public class WheelSlot : MonoBehaviour
{
    public Transform mountPoint;
    public bool isOccupied = false;

    public void CheckOccupiedStatus()
    {
        if (GetComponentInChildren<Tire>() != null)
        {
            isOccupied = true;
        }
        else
        {
            isOccupied = false;
        }
    }
}
