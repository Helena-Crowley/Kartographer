// using LRS;
// using UnityEngine;

// public class VFXContainer : MonoBehaviour
// {
//     private Scanner vfxScanner;

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.gameObject.tag == "Player")
//         {
//             Debug.Log(other.gameObject.name + "alskdlaskd");
//             vfxScanner = other.gameObject.GetComponentInChildren<Scanner>();
//             vfxScanner._vfxContainer = gameObject;
//             vfxScanner.CreateNewVisualEffect();
//             vfxScanner.ApplyPositions();
//             MapToggle mapToggle = other.gameObject.GetComponent<MapToggle>();
//             //mapToggle.buildings = gameObject;

//             Destroy(GetComponent<BoxCollider>());
//         }
//     }

// }
using LRS;
using UnityEngine;

public class VFXContainer : MonoBehaviour
{
    private Scanner vfxScanner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            vfxScanner = other.GetComponentInChildren<Scanner>();
            vfxScanner._vfxContainer = gameObject; // assign building as VFX container
            vfxScanner.CreateNewVisualEffect();
            vfxScanner.ApplyPositions();

            // Dynamically tell ScannerUI which building this is
            ScannerUI scannerUI = other.GetComponent<ScannerUI>();
            if (scannerUI != null)
            {
                scannerUI.currentBuilding = gameObject;
                scannerUI.ShowWelcomeText("Scannable Zone");
            }
            else
            {
                Debug.LogWarning("NO SCANNERUI WAS BINDED");
            }

            Destroy(GetComponent<BoxCollider>()); // optional if scan is one-time
        }
    }
}

