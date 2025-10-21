using UnityEngine;

[CreateAssetMenu(fileName = "VersionData", menuName = "Version/VersionData")]
public class VersionData : ScriptableObject
{
    public int buildNumber = 0;
    public string versionPrefix = "v0.0.";

    public string GetVersionString()
    {
        return versionPrefix + buildNumber;
    }
}
