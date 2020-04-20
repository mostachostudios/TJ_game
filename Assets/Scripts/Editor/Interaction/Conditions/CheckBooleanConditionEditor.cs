using UnityEngine;
using UnityEditor;
using System.Linq;
using System;


[CustomEditor(typeof(CheckBooleanFieldCondition))]
public class CheckBooleanConditionEditor : ConditionEditor
{
    private CheckBooleanFieldCondition checkBooleanCondition;

    private const string instancePropName = "instance";
    private const string componentTypePropName = "componentType";
    private const string fieldPropName = "field";

    private SerializedProperty instanceProperty;
    //private SerializedProperty componentTypeProperty;
    //private SerializedProperty fieldProperty;

    private string[] booleanFieldNamesList = new string[0];
    private int selectedBooleanFieldIndex;

    private string[] componentsNamesList = new string[0];
    private int selectedComponentTypeIndex;

    protected override void Init()
    {
        checkBooleanCondition = (CheckBooleanFieldCondition)target;

        instanceProperty = serializedObject.FindProperty(instancePropName);
        //componentTypeProperty = serializedObject.FindProperty(componentTypePropName);
        //fieldProperty = serializedObject.FindProperty(fieldPropName);

        if (checkBooleanCondition.componentTypeName != null)
        {
            var componentsArray = checkBooleanCondition.instance.GetComponents<Component>().Select(component => component.GetType().Name).ToArray();
            selectedComponentTypeIndex = Array.IndexOf(componentsArray, checkBooleanCondition.componentTypeName);

            if (checkBooleanCondition.fieldName != null)
            {
                var fieldsArray = checkBooleanCondition.instance.GetComponent(checkBooleanCondition.componentTypeName).GetType().GetFields().Where(prop => prop.FieldType == typeof(bool)).Select(prop => prop.Name).ToArray();
                selectedBooleanFieldIndex = Array.FindIndex(fieldsArray, fieldName => fieldName == checkBooleanCondition.fieldName);
            }
            else
            {
                selectedBooleanFieldIndex = 0;
                var fieldsArray = checkBooleanCondition.instance.GetComponent(checkBooleanCondition.componentTypeName).GetType().GetFields().Where(prop => prop.FieldType == typeof(bool)).Select(prop => prop.Name).ToArray();
                if (fieldsArray.Length > 0)
                {
                    checkBooleanCondition.fieldName = checkBooleanCondition.instance.GetComponent(checkBooleanCondition.componentTypeName).GetType().GetFields().Where(prop => prop.FieldType == typeof(bool)).Select(prop => prop.Name).ToArray()[0];
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

    protected override void DrawCondition()
    {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);

        EditorGUILayout.PropertyField(instanceProperty, GUIContent.none);

        GameObject oldInstance = checkBooleanCondition.instance;

        serializedObject.ApplyModifiedProperties();

        if (checkBooleanCondition.instance != oldInstance)
        {
            selectedComponentTypeIndex = 0;
            selectedBooleanFieldIndex = 0;

            checkBooleanCondition.componentTypeName = null;
            checkBooleanCondition.fieldName = null;

            UpdateComponentNamesList();
            UpdateBooleanFieldNamesList();

            var componentsArray = checkBooleanCondition.instance.GetComponents<Component>().Select(component => component.GetType()).ToArray();
            if (componentsArray.Length > 0)
            {
                checkBooleanCondition.componentTypeName = componentsArray[selectedComponentTypeIndex].Name;

                var fieldsArray = checkBooleanCondition.instance.GetComponent(checkBooleanCondition.componentTypeName).GetType().GetFields().Where(prop => prop.FieldType == typeof(bool)).ToArray();
                if (fieldsArray.Length > 0)
                {
                    checkBooleanCondition.fieldName = fieldsArray[selectedBooleanFieldIndex].Name;
                }
            }
            else
            {
                checkBooleanCondition.componentTypeName = null;
                checkBooleanCondition.fieldName = null;
            }
        }

        if (componentsNamesList.Length > 0)
        {
            int oldSelectedComponentTypeIndex = selectedComponentTypeIndex;
            selectedComponentTypeIndex = EditorGUILayout.Popup(selectedComponentTypeIndex, componentsNamesList);

            checkBooleanCondition.componentTypeName = checkBooleanCondition.instance.GetComponents<Component>().Select(component => component.GetType()).ToArray()[selectedComponentTypeIndex].Name;

            if (checkBooleanCondition.componentTypeName == null)
            {
                throw new UnityException("Component type is null");
            }

            if (oldSelectedComponentTypeIndex != selectedComponentTypeIndex)
            {
                checkBooleanCondition.fieldName = null;
                selectedBooleanFieldIndex = 0;
                UpdateBooleanFieldNamesList();
                if (booleanFieldNamesList.Length > 0)
                {
                    checkBooleanCondition.fieldName = checkBooleanCondition.instance.GetComponent(checkBooleanCondition.componentTypeName).GetType().GetFields().Where(prop => prop.FieldType == typeof(bool)).ToArray()[selectedBooleanFieldIndex].Name;
                }
            }
        }

        if (checkBooleanCondition.componentTypeName != null && booleanFieldNamesList.Length > 0)
        {
            int oldSelectedBooleanFieldIndex = selectedBooleanFieldIndex;
            selectedBooleanFieldIndex = EditorGUILayout.Popup(selectedBooleanFieldIndex, booleanFieldNamesList);
            if (oldSelectedBooleanFieldIndex != selectedBooleanFieldIndex || checkBooleanCondition.fieldName == null)
            {
                checkBooleanCondition.fieldName = checkBooleanCondition.instance.GetComponent(checkBooleanCondition.componentTypeName).GetType().GetFields().Where(prop => prop.FieldType == typeof(bool)).ElementAt(selectedBooleanFieldIndex).Name;
            }
        }

        if (checkBooleanCondition.fieldName != null)
        {
            checkBooleanCondition.expectedValue = EditorGUILayout.Toggle(checkBooleanCondition.expectedValue);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void UpdateComponentNamesList()
    {
        if (checkBooleanCondition != null && checkBooleanCondition.instance != null)
        {
            componentsNamesList = checkBooleanCondition.instance.GetComponents<Component>().Select(component => component.GetType().Name).ToArray();
        }
        else
        {
            componentsNamesList = new string[0];
            checkBooleanCondition.componentTypeName = null;
        }
    }

    private void UpdateBooleanFieldNamesList()
    {
        if (componentsNamesList != null && componentsNamesList.Length > 0 && checkBooleanCondition.componentTypeName != null)
        {
            var component = checkBooleanCondition.instance.GetComponent(checkBooleanCondition.componentTypeName);
            booleanFieldNamesList = component.GetType().GetFields().Where(prop => prop.FieldType == typeof(bool)).Select(prop => prop.Name).ToArray();
        }
        else
        {
            booleanFieldNamesList = new string[0];
            checkBooleanCondition.fieldName = null;
        }
    }
}
