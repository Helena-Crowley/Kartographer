using UnityEditor;
using UnityEngine;

public class ToggleMeshReadWrite
{
    [MenuItem("Tools/Toggle ReadWrite")]
    static void ToggleSelectedMesh()
    {
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        var importer = AssetImporter.GetAtPath(path) as ModelImporter;

        if (importer != null)
        {
            importer.isReadable = !importer.isReadable;
            Debug.Log($"Toggled Read/Write to {importer.isReadable} for {path}");
            importer.SaveAndReimport();
        }
        else
        {
            Debug.LogWarning("Selected asset is not a model with a mesh importer.");
        }
    }
}
