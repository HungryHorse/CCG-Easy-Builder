using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class StartUp
{
    static StartUp()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.hierarchyChanged += HierarchyWindowChanged;
        }
    }

    static void HierarchyWindowChanged()
    {
        try
        {
            RunOnceOnLoad();
        }
        catch
        {
        }
    }

    static void RunOnceOnLoad()
    {
        GameObject.Find("Settings").GetComponent<Settings>().LoadFromFile();
        EditorApplication.update -= RunOnceOnLoad;
    }
}
