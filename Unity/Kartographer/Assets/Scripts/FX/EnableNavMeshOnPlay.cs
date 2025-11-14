using Unity.AI.Navigation;
using UnityEngine;

public class EnableNavMeshOnPlay : MonoBehaviour
{
    [SerializeField] private NavMeshSurface nav;
    void Start()
    {
        nav.enabled = true;
    }
}
