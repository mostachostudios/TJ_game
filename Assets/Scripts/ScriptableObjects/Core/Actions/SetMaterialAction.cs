using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMaterialAction : Action
{
    public MeshRenderer meshRenderer;
    public int materialIndex;
    public Material material;

    protected override bool StartDerived()
    {
        Material[] materials = meshRenderer.materials;
        materials[materialIndex] = material;      
        meshRenderer.materials = materials;
        return true;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    protected override Action CloneDerived()
    {
        SetMaterialAction clone = ScriptableObject.CreateInstance<SetMaterialAction>();

        return clone;
    }
}
