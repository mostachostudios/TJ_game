using UnityEngine;

public class SetEnableDisableComponentAction : Action
{
    public Component component;
    public bool enable;

    protected override bool StartDerived()
    {
        if (component)
        {
            System.Type type = component.GetType();
            var propertyEnabled = type.GetProperty("enabled");
            if (propertyEnabled != null)
            {
                propertyEnabled.SetValue(component, enable);
            }
            else
            {
                Debug.Log("It seems '" + component + "' does not have 'enabled' property.");
            }
        }

        return true;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    protected override Action CloneDerived()
    {
        SetEnableDisableComponentAction clone = ScriptableObject.CreateInstance<SetEnableDisableComponentAction>();
        clone.component = this.component;
        clone.enable = this.enable;

        return clone;
    }
}
