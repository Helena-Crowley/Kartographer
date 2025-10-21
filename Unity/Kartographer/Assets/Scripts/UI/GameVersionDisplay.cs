using UnityEngine;
using TMPro; // or UnityEngine.UI

public class MenuVersionDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text versionText;

    void Start()
    {
        VersionData version = Resources.Load<VersionData>("VersionData");
        versionText.text = version.GetVersionString();
    }
}
