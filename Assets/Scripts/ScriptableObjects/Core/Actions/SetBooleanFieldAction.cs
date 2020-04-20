using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBooleanFieldAction : Action
{
    public GameObject instance;
    public string componentTypeName;
    public string fieldName;
    public bool value;

    protected override bool StartDerived()
    {
        if (instance == null || componentTypeName == null || fieldName == null)
        {
            return false;
        }

        Component component = instance.GetComponent(componentTypeName);
        System.Type type = component.GetType();
        type.GetField(fieldName).SetValue(component, value);

        return true;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    protected override Action CloneDerived()
    {
        SetBooleanFieldAction clone = ScriptableObject.CreateInstance<SetBooleanFieldAction>();

        clone.instance = this.instance;
        clone.componentTypeName = this.componentTypeName;
        clone.fieldName = this.fieldName;
        clone.value = this.value;

        return clone;
    }
}
