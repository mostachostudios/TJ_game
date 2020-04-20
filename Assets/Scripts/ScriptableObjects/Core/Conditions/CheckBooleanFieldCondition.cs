using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBooleanFieldCondition : Condition 
{
    public GameObject instance;
    public string componentTypeName;
    public string fieldName;
    public bool expectedValue;


    public override bool Check()
    {
        if(instance == null || componentTypeName == null || fieldName == null)
        {
            return false;
        }

        Component component = instance.GetComponent(componentTypeName);
        System.Type type = component.GetType();
        return ((bool) type.GetField(fieldName).GetValue(component)) == expectedValue;
    }

    public override Condition Clone()
    {
        CheckBooleanFieldCondition clone = ScriptableObject.CreateInstance<CheckBooleanFieldCondition>();

        clone.instance = this.instance;
        clone.componentTypeName = this.componentTypeName;
        clone.fieldName = this.fieldName;
        clone.expectedValue = this.expectedValue;

        return clone;
    }
}
