using UnityEditor;
using UnityEngine;

public class VersionIncrementer
{
    public static void IncrementBuildNumber()
    {
        var version = AssetDatabase.LoadAssetAtPath<VersionData>("Assets/Resources/VersionData.asset");
        if (version == null)
        {
            Debug.LogError("VersionData asset not found!");
            return;
        }

        version.buildNumber++;
        EditorUtility.SetDirty(version);
        AssetDatabase.SaveAssets();
        Debug.Log("Version incremented: " + version.buildNumber);
    }
}
