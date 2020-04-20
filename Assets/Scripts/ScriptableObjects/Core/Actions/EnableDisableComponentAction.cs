using UnityEngine;

public class EnableDisableComponentAction : Action
{
    public MonoBehaviour component;
    public bool enable = false;

    protected override bool StartDerived()
    {
        if (component == null)
        {
            throw new UnityEngine.UnityException("Component is null");
        }

        component.enabled = enable;

        return true;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    protected override Action CloneDerived()
    {
        EnableDisableComponentAction clone = ScriptableObject.CreateInstance<EnableDisableComponentAction>();
        clone.component = this.component;
        clone.enable = this.enable;

        return clone;
    }
}
