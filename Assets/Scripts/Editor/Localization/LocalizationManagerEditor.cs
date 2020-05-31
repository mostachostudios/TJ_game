using System;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

[CustomEditor(typeof(LocalizationManager), true)]
public class LocalizationManagerEditor : Editor
{
    public LocalizationManager localizationManager;

    private void OnEnable()
    {
        localizationManager = target as LocalizationManager;
    }


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Reload CSV files"))
        {
            localizationManager.ReloadMetadata();
        }
    }

}
