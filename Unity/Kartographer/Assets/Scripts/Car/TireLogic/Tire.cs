using UnityEngine;

public class Tire : MonoBehaviour, IInteractable
{
    [SerializeField] private float dropDistance;

    private float tireWidth;

    //overriden function from iinteractable
    public void Interact(PlayerInteractor player)
    {
        Debug.Log("Movinf object to players hand");
        transform.SetParent(player.handPosition);
        transform.position = player.handPosition.position;
    }

    //overriden function from iinteractable
    public void Drop(PlayerInteractor player)
    {
        transform.SetParent(null);
        if (Physics.Raycast(transform.position + player.transform.forward * dropDistance, -Vector3.up, out RaycastHit hit))
        {
            transform.position = hit.point + new Vector3(0, tireWidth, 0);
        }
    }

    private void Start()
    {
        tireWidth = GetComponent<Renderer>().bounds.size.y;
    }
}
