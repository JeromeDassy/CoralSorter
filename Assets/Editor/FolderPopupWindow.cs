using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class FolderPopupWindow : EditorWindow
{
    private Action<string> onSelectFolder;
    private List<string> rootFolders = new List<string>();
    private Dictionary<string, List<string>> folderHierarchy = new Dictionary<string, List<string>>();
    private Dictionary<string, bool> expandedFolders = new Dictionary<string, bool>();

    private Vector2 scrollPos;

    public static void ShowWindow(Rect rect, string rootFolder, Action<string> onSelect)
    {
        FolderPopupWindow window = ScriptableObject.CreateInstance<FolderPopupWindow>();
        window.onSelectFolder = onSelect;
        window.LoadFolders(rootFolder);

        // Adjust these values as needed
        float windowHeight = 300f;
        float windowWidth = rect.width;

        // Calculate the screen position for the dropdown
        Vector2 screenPosition = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.yMax));

        // Show the dropdown as a popup below the control
        window.ShowAsDropDown(new Rect(screenPosition.x, screenPosition.y, rect.width, windowHeight), new Vector2(windowWidth, windowHeight));
    }

    private void LoadFolders(string rootFolder)
    {
        if (Directory.Exists(rootFolder))
        {
            string[] directories = Directory.GetDirectories(rootFolder, "*", SearchOption.AllDirectories);
            foreach (string directory in directories)
            {
                string relativePath = directory.Replace("\\", "/").Substring("Assets/Resources/".Length);
                string parentPath = Path.GetDirectoryName(relativePath).Replace("\\", "/");

                if (!folderHierarchy.ContainsKey(parentPath))
                {
                    folderHierarchy[parentPath] = new List<string>();
                }
                folderHierarchy[parentPath].Add(relativePath);

                if (!folderHierarchy.ContainsKey(""))
                {
                    folderHierarchy[""] = new List<string>();
                }

                if (parentPath == "")
                {
                    rootFolders.Add(relativePath);
                }

                expandedFolders[relativePath] = false;
            }
        }
    }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        DrawFolders("", 0);
        EditorGUILayout.EndScrollView();
    }

    private void DrawFolders(string parentPath, int indentLevel)
    {
        if (!folderHierarchy.ContainsKey(parentPath)) return;

        foreach (string folder in folderHierarchy[parentPath])
        {
            bool hasSubFolders = folderHierarchy.ContainsKey(folder);
            bool isExpanded = expandedFolders.ContainsKey(folder) && expandedFolders[folder];

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentLevel * 15);

            if (hasSubFolders)
            {
                expandedFolders[folder] = EditorGUILayout.Foldout(isExpanded, "");
            }
            else
            {
                GUILayout.Space(15); // Indentation for folders without children
            }

            // Extract the folder name from the path
            string folderName = Path.GetFileName(folder);

            // Display the folder name, aligned to the left
            if (GUILayout.Button(folderName, EditorStyles.label))
            {
                onSelectFolder?.Invoke(folder);
                Close();
            }

            EditorGUILayout.EndHorizontal();

            if (isExpanded)
            {
                DrawFolders(folder, indentLevel + 1);
            }
        }
    }
}
