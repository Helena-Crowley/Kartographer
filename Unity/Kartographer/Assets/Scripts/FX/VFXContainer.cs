using LRS;
using UnityEngine;

public class VFXContainer : MonoBehaviour
{
    private Scanner vfxScanner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log(other.gameObject.name + "alskdlaskd");
            vfxScanner = other.gameObject.GetComponentInChildren<Scanner>();
            vfxScanner._vfxContainer = gameObject;
            vfxScanner.CreateNewVisualEffect();
            vfxScanner.ApplyPositions();

            Destroy(GetComponent<BoxCollider>());
        }
    }

}
