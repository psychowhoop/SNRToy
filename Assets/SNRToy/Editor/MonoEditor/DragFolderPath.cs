using System;
using System.Reflection;
using SNRLogHelper;
using UILayerManager;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This class provides an extension for specifying a script class in the Unity editor.
/// It allows dragging a folder to get the folder path.
/// </summary>
public class DragFolderPath<T> : Editor where T : MonoBehaviour
{
    // The name of the field in the target script where the folder path will be stored.
    public string _pathFieldName;
    // The name of the root folder. If specified, only the subfolders under this root folder will be considered.
    public string rootFolderName = null;

    // Method to automatically find the default path field name by searching for fields containing "path" in their name.
    private void GetDefaultPathFieldName()
    {
        Type targetType = typeof(T);
        FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(string) && field.Name.ToLower().Contains("path"))
            {
                _pathFieldName = field.Name;
                break;
            }
        }
    }

    // Override the OnInspectorGUI method to customize the inspector GUI.
    public override void OnInspectorGUI()
    {
        // Get the target script instance.
        T script = (T)target;

        // Draw the default inspector GUI.
        DrawDefaultInspector();

        // Add some space.
        GUILayout.Space(10);

        // If _pathFieldName is empty, automatically find the default path field name.
        if (string.IsNullOrEmpty(_pathFieldName))
            GetDefaultPathFieldName();

        // Display the input field for specifying the path field name.
        _pathFieldName = EditorGUILayout.TextField("Path field Name", _pathFieldName);

        // Add some space.
        GUILayout.Space(10);

        // Display the drop area for dragging folders.
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag & Drop Here");

        // Handle drag and drop events.
        Event curEvent = Event.current;
        switch (curEvent.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                {
                    // Check if the mouse is within the drop area.
                    if (!dropArea.Contains(curEvent.mousePosition))
                        return;

                    // Set visual mode to indicate copy.
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    // Check if it's a drag perform event.
                    if (curEvent.type == EventType.DragPerform)
                    {
                        // Accept the drag.
                        DragAndDrop.AcceptDrag();

                        // Iterate through each dragged object.
                        foreach (UnityEngine.Object dragObj in DragAndDrop.objectReferences)
                        {
                            // Check if it's a folder.
                            if (dragObj is DefaultAsset)
                            {
                                Type scType = typeof(T);
                                FieldInfo fInfo = scType.GetField(_pathFieldName);

                                // Check if the field exists in the target script.
                                if (fInfo != null)
                                {
                                    string folderPath = AssetDatabase.GetAssetPath(dragObj);
                                    int folderIdx = folderPath.IndexOf("AllToHot");
                                    if (folderIdx < 0)
                                    {
                                        folderIdx = folderPath.IndexOf("Assets");
                                    }
                                    string usePath = "/" + folderPath.Substring(folderIdx) + "/";
                                    // Assign the folder path to the field.
                                    fInfo.SetValue(script, usePath);

                                }
                                else
                                {
                                    SLog.Err($"field {_pathFieldName} not found in script");
                                }
                            }
                        }
                    }
                }
                break;

        }
    }
}

#region the class which need add this extension

// Custom editor for SpriteManager class that uses the DragFolderPath extension.
[CustomEditor(typeof(SpriteManager))]
public class SpriteManagerEditor : DragFolderPath<SpriteManager>
{

}

// Custom editor for LayerManager class that uses the DragFolderPath extension.
// [CustomEditor(typeof(LayerManager))]
// public class LayerManagerExt : DragFolderPath<LayerManager>
// {

// }

#endregion
