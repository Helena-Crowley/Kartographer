using UnityEngine;

public class TireAttach : MonoBehaviour
{
    [SerializeField] private tireEmpty;
    public void AttachTireMesh(GameObject tire)
    {
        tire.transform.SetParent(tireEmpty.transform);
        tire.transform.position = transform.position;
    }
}
