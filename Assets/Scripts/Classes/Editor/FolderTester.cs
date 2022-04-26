using UnityEngine;
using UnityEditor;

class FolderTester : EditorWindow
{

    private DefaultAsset targetFolder = null;

    [MenuItem("Window/Folder Selection Example")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FolderTester));
    }

    void OnGUI()
    {
        targetFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Select Folder",
            targetFolder,
            typeof(DefaultAsset),
            false);

        if (targetFolder != null)
        {
            EditorGUILayout.HelpBox(
                "Valid folder! Name: " + targetFolder.name,
                MessageType.Info,
                true);
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Not valid!",
                MessageType.Warning,
                true);
        }

    }
}