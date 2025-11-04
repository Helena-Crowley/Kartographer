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
using System.Collections;
using LRS;
using TMPro;
using UnityEngine;

public class VFXContainer : MonoBehaviour
{
    public TMP_Text scannableZoneText;

    private Scanner scanner;
    private bool isFading = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFading)
        {
            ShowWelcomeText("Scannable Zone");
            
            scanner = other.GetComponentInChildren<Scanner>();
            scanner._vfxContainer = gameObject; // assign building as VFX container
            // scanner.CreateNewVisualEffect();
            // scanner.ApplyPositions();

            // Dynamically tell ScannerUI which building this is
            ScannerUI scannerUI = other.GetComponent<ScannerUI>();
            if (scannerUI != null)
            {
                scannerUI.currentBuilding = gameObject;
                scannerUI.vfxContainer = this;
            }
            else
            {
                Debug.LogWarning("NO SCANNERUI WAS BINDED");
            }
        }
    }

    public void ShowWelcomeText(string message = "Scannable Zone")
    {
        if (isFading) return; // prevent multiple starts
        isFading = true;

        scannableZoneText.text = message;
        StartCoroutine(FadeTextCoroutine(scannableZoneText, 2f));
    }

    private IEnumerator FadeTextCoroutine(TMP_Text text, float fadeDuration)
    {
        text.gameObject.SetActive(true);
        Color originalColor = text.color;

        // Fade in
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Hold fully visible
        yield return new WaitForSeconds(1f);

        // Fade out
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(timer / fadeDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        text.gameObject.SetActive(false);
        text.color = originalColor;
        isFading = false; // reset flag when done
    }

}

