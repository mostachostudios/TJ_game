using UnityEngine;
using UnityEditor;

public abstract class EditorWithSubEditorsTwoTypes31<TEditor1, TTarget1, TEditor2, TTarget2> : Editor
    where TEditor1 : Editor
    where TTarget1 : Object
    where TEditor2 : Editor
    where TTarget2 : Object
{
    protected TEditor1[][] subEditors1 = new TEditor1[3][];
    protected TEditor2[] subEditors2;

    public void CheckAndCreateSubEditors(TTarget1[] subEditorTargets11, TTarget1[] subEditorTargets12, TTarget1[] subEditorTargets13, TTarget2[] subEditorTargets2)
    {
        CheckAndCreateSubEditors(subEditorTargets11, 0);
        CheckAndCreateSubEditors(subEditorTargets12, 1);
        CheckAndCreateSubEditors(subEditorTargets13, 2);
        CheckAndCreateSubEditors(subEditorTargets2);
    }

    public void CleanupEditors()
    {
        CleanupEditors1(0);
        CleanupEditors1(1);
        CleanupEditors1(2);

        subEditors1 = null;

        CleanupEditors2();
    }

    private void CheckAndCreateSubEditors(TTarget1[] subEditorTargets, int index)
    {
        if (subEditors1 != null && subEditors1[index] != null && subEditors1[index].Length == subEditorTargets.Length)
        {
            return;
        }

        CleanupEditors1(index);

        if(subEditors1 == null)
        {
            subEditors1 = new TEditor1[3][];
        }

        subEditors1[index] = new TEditor1[subEditorTargets.Length];

        for (int i = 0; i < subEditors1[index].Length; i++)
        {
            subEditors1[index][i] = CreateEditor(subEditorTargets[i] as TTarget1) as TEditor1;
            SubEditorSetup(subEditors1[index][i]);
        }
    }

    private void CheckAndCreateSubEditors(TTarget2[] subEditorTargets)
    {
        if (subEditors2 != null && subEditors2.Length == subEditorTargets.Length)
        {
            return;
        }

        CleanupEditors2();

        subEditors2 = new TEditor2[subEditorTargets.Length];

        for (int i = 0; i < subEditors2.Length; i++)
        {
            subEditors2[i] = CreateEditor(subEditorTargets[i] as TTarget2) as TEditor2;
            SubEditorSetup(subEditors2[i]);
        }
    }

    private void CleanupEditors1(int index)
    {
        if (subEditors1 == null || subEditors1[index] == null)
            return;

        for (int i = 0; i < subEditors1[index].Length; i++)
        {
            DestroyImmediate(subEditors1[index][i]);
        }

        subEditors1[index] = null;
    }

    private void CleanupEditors2()
    {
        if (subEditors2 == null)
            return;

        for (int i = 0; i < subEditors2.Length; i++)
        {
            DestroyImmediate(subEditors2[i]);
        }

        subEditors2 = null;
    }


    public abstract void SubEditorSetup(TEditor1 editor);
    public abstract void SubEditorSetup(TEditor2 editor);
}