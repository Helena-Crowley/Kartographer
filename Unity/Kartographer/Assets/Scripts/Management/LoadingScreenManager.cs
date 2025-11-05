// using UnityEngine;
// using UnityEngine.UI;

// public class LoadingScreenManager : MonoBehaviour
// {
//     public static LoadingScreenManager Instance;

//     public GameObject loadingScreen;
//     public Image loadingBarFill;

//     private void Awake()
//     {
//         // Singleton pattern
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }
//         Instance = this;

//         // Keep this object alive across scene loads
//         DontDestroyOnLoad(gameObject);

//         loadingScreen.SetActive(false);
//         loadingBarFill.fillAmount = 0f;
//     }

//     public void Show()
//     {
//         loadingScreen.SetActive(true);
//     }

//     public void Hide()
//     {
//         loadingScreen.SetActive(false);
//         loadingBarFill.fillAmount = 0f;
//     }

//     public void SetProgress(float value)
//     {
//         loadingBarFill.fillAmount = value;
//     }
// }
