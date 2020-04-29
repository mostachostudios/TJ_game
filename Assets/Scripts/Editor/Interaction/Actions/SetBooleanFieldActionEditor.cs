using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[CustomEditor(typeof(SetBooleanFieldAction))]
public class SetBooleanFieldActionEditor : ActionEditor
{
    private SetBooleanFieldAction setBooleanAction;

    private const string instancePropName = "instance";
    private const string componentTypePropName = "componentType";
    private const string fieldPropName = "field";

    private SerializedProperty instanceProperty;

    private string[] booleanFieldNamesList = new string[0];
    private int selectedBooleanFieldIndex;

    private string[] componentsNamesList = new string[0];
    private int selectedComponentTypeIndex;

    protected override void Init()
    {
        setBooleanAction = (SetBooleanFieldAction)target;

        instanceProperty = serializedObject.FindProperty(instancePropName);
        //componentTypeProperty = serializedObject.FindProperty(componentTypePropName);
        //fieldProperty = serializedObject.FindProperty(fieldPropName);

        if (setBooleanAction.componentTypeName != null)
        {
            var componentsArray = setBooleanAction.instance.GetComponents<Component>().Select(component => component.GetType().Name).ToArray();
            selectedComponentTypeIndex = Array.IndexOf(componentsArray, setBooleanAction.componentTypeName);

            if (setBooleanAction.fieldName != null)
            {
                var fieldsArray = setBooleanAction.instance.GetComponent(setBooleanAction.componentTypeName).GetType().GetFields().Where(prop => prop.FieldType == typeof(bool)).Select(prop => prop.Name).ToArray();
                selectedBooleanFieldIndex = Array.FindIndex(fieldsArray, fieldName => fieldName == setBooleanAction.fieldName);
            }
            else
            {
                selectedBooleanFieldIndex = 0;
                var fieldsArray = setBooleanAction.instance.GetComponent(setBooleanAction.componentTypeName).GetType().GetFields().Where(prop => prop.FieldType == typeof(bool)).Select(prop => prop.Name).ToArray();
                if (fieldsArray.Length > 0)
                {
                    setBooleanAction.fieldName = setBooleanAction.instance.GetComponent(setBooleanAction.componentTypeName).GetType().GetFields().Where(prop => prop.FieldType == typeof(bool)).Select(prop => prop.Name).ToArray()[0];
                }
            }
        }
        else
        {
            selectedComponentTypeIndex = 0;
            selectedBooleanFieldIndex = 0;
        }

        UpdateComponentNamesList();
        UpdateBooleanFieldNamesList();
    }

    protected override void DrawAction()
    {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);

        EditorGUILayout.PropertyField(instanceProperty, GUIContent.none);

        GameObject oldInstance = setBooleanAction.instance;

        serializedObject.ApplyModifiedProperties();

        if (setBooleanAction.instance != oldInstance)
        {
            selectedComponentTypeIndex = 0;
            selectedBooleanFieldIndex = 0;

            setBooleanAction.componentTypeName = null;
            setBooleanAction.fieldName = null;

            UpdateComponentNamesList();
            UpdateBooleanFieldNamesList();

            var componentsArray = setBooleanAction.instance.GetComponents<Component>().Select(component => component.GetType()).ToArray();
            if (componentsArray.Length > 0)
            {
                setBooleanAction.componentTypeName = componentsArray[selectedComponentTypeIndex].Name;

                var fieldsArray = setBooleanAction.instance.GetComponent(setBooleanAction.componentTypeName).GetType().GetFields().Where(prop => prop.FieldType == typeof(bool)).ToArray();
                if (fieldsArray.Length > 0)
                {
                    setBooleanAction.fieldName = fieldsArray[selectedBooleanFieldIndex].Name;
                }
            }
            else
            {
                setBooleanAction.componentTypeName = null;
                setBooleanAction.fieldName = null;
            }
        }

        if (componentsNamesList.Length > 0)
        {
            int oldSelectedComponentTypeIndex = selectedComponentTypeIndex;
            selectedComponentTypeIndex = EditorGUILayout.Popup(selectedComponentTypeIndex, componentsNamesList);

            setBooleanAction.componentTypeName = setBooleanAction.instance.GetComponents<Component>().Select(component => component.GetType()).ToArray()[selectedComponentTypeIndex].Name;

            if (setBooleanAction.componentTypeName == null)
            {
                throw new UnityException("Component type is null");
            }

            if (oldSelectedComponentTypeIndex != selectedComponentTypeIndex)
            {
                setBooleanAction.fieldName = null;
                selectedBooleanFieldIndex = 0;
                UpdateBooleanFieldNamesList();
                if (booleanFieldNamesList.Length > 0)
                {
                    setBooleanAction.fieldName = setBooleanAction.instance.GetComponent(setBooleanAction.componentTypeName).GetType().GetFields().Where(prop => prop.FieldType == typeof(bool)).ToArray()[selectedBooleanFieldIndex].Name;
                }
            }
        }

        if (setBooleanAction.componentTypeName != null && booleanFieldNamesList.Length > 0)
        {
            int oldSelectedBooleanFieldIndex = selectedBooleanFieldIndex;
            selectedBooleanFieldIndex = EditorGUILayout.Popup(selectedBooleanFieldIndex, booleanFieldNamesList);
            if (oldSelectedBooleanFieldIndex != selectedBooleanFieldIndex || setBooleanAction.fieldName == null)
            {
                setBooleanAction.fieldName = setBooleanAction.instance.GetComponent(setBooleanAction.componentTypeName).GetType().GetFields().Where(prop => prop.FieldType == typeof(bool)).ElementAt(selectedBooleanFieldIndex).Name;
            }
        }

        if (setBooleanAction.fieldName != null)
        {
            setBooleanAction.value = EditorGUILayout.Toggle(setBooleanAction.value);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void UpdateComponentNamesList()
    {
        if (setBooleanAction != null && setBooleanAction.instance != null)
        {
            componentsNamesList = setBooleanAction.instance.GetComponents<Component>().Select(component => component.GetType().Name).ToArray();
        }
        else
        {
            componentsNamesList = new string[0];
            setBooleanAction.componentTypeName = null;
        }
    }

    private void UpdateBooleanFieldNamesList()
    {
        if (componentsNamesList != null && componentsNamesList.Length > 0 && setBooleanAction.componentTypeName != null)
        {
            var component = setBooleanAction.instance.GetComponent(setBooleanAction.componentTypeName);
            booleanFieldNamesList = component.GetType().GetFields().Where(prop => prop.FieldType == typeof(bool)).Select(prop => prop.Name).ToArray();
        }
        else
        {
            booleanFieldNamesList = new string[0];
            setBooleanAction.fieldName = null;
        }
    }
}