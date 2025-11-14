using UnityEngine;

public class Tire : MonoBehaviour, IInteractable
{
    [SerializeField] private float dropDistance;
    [SerializeField] private AudioClip pickUpSound;
    [SerializeField] private AudioClip dropSound;

    private float tireWidth;

    //overriden function from iinteractable
    public void Interact(PlayerInteractor player)
    {
        Debug.Log("Movinf object to players hand");
        transform.SetParent(player.handPosition);
        transform.position = player.handPosition.position;
        GetComponent<MeshCollider>().isTrigger = true;

        SoundManager.Instance.PlaySound(pickUpSound, transform.position, "SFX", 0.2f, true);
    }

    //overriden function from iinteractable
    public void Drop(PlayerInteractor player)
    {
        transform.SetParent(null);
        if (Physics.Raycast(transform.position + player.transform.forward * dropDistance, -Vector3.up, out RaycastHit hit))
        {
            transform.position = hit.point + new Vector3(0, tireWidth, 0);
            SoundManager.Instance.PlaySound(dropSound, transform.position, "SFX", 0.2f, true);

        }
        GetComponent<MeshCollider>().isTrigger = false;

    }

    private void Start()
    {
        tireWidth = GetComponent<Renderer>().bounds.size.y;
    }
}
