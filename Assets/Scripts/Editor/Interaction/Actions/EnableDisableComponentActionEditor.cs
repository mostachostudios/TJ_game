using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[CustomEditor(typeof(EnableDisableComponentAction))]
public class EnableDisableComponentActionEditor : ActionEditor
{
    EnableDisableComponentAction enableDisableComponentAction;

    private const string componentPropName = "component";
    private const string enablePropName = "enable";

    private GameObject instance;

    private SerializedProperty componentProperty;
    private SerializedProperty enableProperty;

    private string[] componentsNamesList = new string[0];
    private int selectedComponentTypeIndex;

    protected override void Init()
    {
        enableDisableComponentAction = (EnableDisableComponentAction)target;

        componentProperty = serializedObject.FindProperty(componentPropName);
        enableProperty = serializedObject.FindProperty(enablePropName);

        if (enableDisableComponentAction.component != null)
        {
            instance = enableDisableComponentAction.component.gameObject;
            MonoBehaviour[] componentsArray = instance.GetComponents<MonoBehaviour>();
            if (componentsArray.Length > 0)
            {
                selectedComponentTypeIndex = Array.IndexOf(componentsArray, enableDisableComponentAction.component);
            }
        }
        else
        {
            selectedComponentTypeIndex = 0;
        }

        UpdateComponentNamesList();
    }

    protected override void DrawAction()
    {
        EditorGUILayout.BeginHorizontal(GUI.skin.box);

        GameObject oldInstance = instance;

        instance = (GameObject) EditorGUILayout.ObjectField(instance, typeof(GameObject), true);

        // If instance changed, update component
        if (oldInstance != instance)
        {
            selectedComponentTypeIndex = 0;

            UpdateComponentNamesList();

            enableDisableComponentAction.component = componentsNamesList.Length > 0 ? instance.GetComponents<MonoBehaviour>()[0] : null;
        }

        serializedObject.Update();

        if (instance != null && componentsNamesList.Length > 0)
        {
            int oldselectedComponentTypeIndex = selectedComponentTypeIndex;
            selectedComponentTypeIndex = EditorGUILayout.Popup(selectedComponentTypeIndex, componentsNamesList);
            if (oldselectedComponentTypeIndex != selectedComponentTypeIndex)
            {
                enableDisableComponentAction.component = instance.GetComponents<MonoBehaviour>()[selectedComponentTypeIndex];
            }
        }

        if (enableDisableComponentAction.component != null)
        {
            EditorGUILayout.PropertyField(enableProperty, GUIContent.none);
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

    }

    private void UpdateComponentNamesList()
    {
        if (instance != null)
        {
            componentsNamesList = instance.GetComponents<MonoBehaviour>().Select(component => component.GetType().Name).ToArray();
        }
    }
}
