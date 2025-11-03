// using LRS;
// using UnityEngine;
// using UnityEngine.UI;
// using Unity.Netcode;
// using TMPro;
// using System.Collections;

// public class ScannerUI : NetworkBehaviour
// {
//     [HideInInspector] public Image percentageSlider;
//     [HideInInspector] public TMP_Text completedText;
//     [HideInInspector] public TMP_Text scanningZoneText;
//     public GameObject currentBuilding;

//     public event System.Action<GameObject> ScanCompleteEvent;
//     private Color originalColor;

//     [SerializeField] private Scanner scanner;
//     void Start()
//     {
//         PlayerUIManager.Instance.BindPlayer(this);
//         originalColor = percentageSlider.color;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         percentageSlider.fillAmount = scanner.scannedPercentage;
//     }

//     public void ScanComplete()
//     {
//         if (!scanner.isScanning)
//         {
//             ScanCompleteEvent?.Invoke(currentBuilding);
//             Debug.Log("Invoked!");
//             StartCoroutine(ShowScanCompleteUI());
//         }
//     }

//     public void ResetUI()
//     {
//         if (percentageSlider != null)
//         {
//             percentageSlider.gameObject.SetActive(true);
//             percentageSlider.color = originalColor; // original color
//             percentageSlider.fillAmount = 0f;
//         }

//         if (completedText != null)
//         {
//             completedText.gameObject.SetActive(false);
//         }
//     }


//     private IEnumerator ShowScanCompleteUI()
//     {
//         // Turn fill green
//         percentageSlider.color = new Color32(128, 255, 0, 255);

//         // Make text fully transparent
//         completedText.color = new Color(completedText.color.r, completedText.color.g, completedText.color.b, 0f);
//         completedText.gameObject.SetActive(true);

//         // Fade in text over 1 second
//         float fadeTime = 1f;
//         float timer = 0f;
//         while (timer < fadeTime)
//         {
//             timer += Time.deltaTime;
//             float alpha = Mathf.Clamp01(timer / fadeTime);
//             completedText.color = new Color(completedText.color.r, completedText.color.g, completedText.color.b, alpha);
//             yield return null;
//         }

//         // Keep UI for 3 seconds
//         yield return new WaitForSeconds(3f);

//         timer = 0f;
//         Color32 startFill = percentageSlider.color;
//         Color textColor = completedText.color;

//         while (timer < fadeTime)
//         {
//             timer += Time.deltaTime;
//             float alpha = 1f - Mathf.Clamp01(timer / fadeTime);
//             completedText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
//             percentageSlider.color = new Color32(
//                 startFill.r,
//                 startFill.g,
//                 startFill.b,
//                 (byte)(255 * alpha)
//             );
//             yield return null;
//         }

//         // Remove everything
//         percentageSlider.color = new Color32(0, 0, 0, 0); // or original color
//         completedText.gameObject.SetActive(false);
//         percentageSlider.gameObject.SetActive(false);
//     }
// }
using LRS;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using System.Collections;

public class ScannerUI : NetworkBehaviour
{
    [HideInInspector] public Image percentageSlider;
    [HideInInspector] public TMP_Text completedText;
    [HideInInspector] public TMP_Text scanningZoneText; // For trigger zone message
    public GameObject currentBuilding;

    public event System.Action<GameObject> ScanCompleteEvent;
    private Color originalColor;

    [SerializeField] private Scanner scanner;

    void Start()
    {
        PlayerUIManager.Instance.BindPlayer(this);
        originalColor = percentageSlider.color;
        if (scanningZoneText != null)
            scanningZoneText.gameObject.SetActive(false); // start hidden
    }

    void Update()
    {
        percentageSlider.fillAmount = scanner.scannedPercentage;
    }

    public void ScanComplete()
    {
        if (!scanner.isScanning)
        {
            ScanCompleteEvent?.Invoke(currentBuilding);
            Debug.Log("Invoked!");
            StartCoroutine(ShowScanCompleteUI());
        }
    }

    public void ResetUI()
    {
        if (percentageSlider != null)
        {
            percentageSlider.gameObject.SetActive(true);
            percentageSlider.color = originalColor;
            percentageSlider.fillAmount = 0f;
        }

        if (completedText != null)
            completedText.gameObject.SetActive(false);

        if (scanningZoneText != null)
            scanningZoneText.gameObject.SetActive(false);
    }

    // New method to show a welcome/scannable zone message
    public void ShowWelcomeText(string message = "Scannable Zone")
    {
        if (scanningZoneText != null)
        {
            scanningZoneText.text = message;
            StartCoroutine(FadeTextCoroutine(scanningZoneText, 2f)); // fade duration 2s
        }
    }

    private IEnumerator FadeTextCoroutine(TMPro.TMP_Text text, float fadeDuration)
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

        // Keep fully visible for 1 second
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
        text.color = originalColor; // reset
    }

    private IEnumerator ShowScanCompleteUI()
    {
        percentageSlider.color = new Color32(128, 255, 0, 255);
        completedText.color = new Color(completedText.color.r, completedText.color.g, completedText.color.b, 0f);
        completedText.gameObject.SetActive(true);

        float fadeTime = 1f;
        float timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeTime);
            completedText.color = new Color(completedText.color.r, completedText.color.g, completedText.color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        timer = 0f;
        Color32 startFill = percentageSlider.color;
        Color textColor = completedText.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(timer / fadeTime);
            completedText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            percentageSlider.color = new Color32(
                startFill.r,
                startFill.g,
                startFill.b,
                (byte)(255 * alpha)
            );
            yield return null;
        }

        percentageSlider.color = new Color32(0, 0, 0, 0);
        completedText.gameObject.SetActive(false);
        percentageSlider.gameObject.SetActive(false);
    }
}
