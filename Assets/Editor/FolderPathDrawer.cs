using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FolderPath))]
public class FolderPathDrawer : PropertyDrawer
{
    private SerializedProperty pathProp;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        pathProp = property.FindPropertyRelative("path");

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        if (GUI.Button(position, string.IsNullOrEmpty(pathProp.stringValue) ? "Select Folder" : pathProp.stringValue, EditorStyles.popup))
        {
            string resourcesRoot = "Assets/Resources";
            Rect rect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, 0);
            FolderPopupWindow.ShowWindow(rect, resourcesRoot, OnFolderSelected);
        }
    }

    private void OnFolderSelected(string relativePath)
    {
        pathProp.stringValue = relativePath;
        pathProp.serializedObject.ApplyModifiedProperties();
    }
}
