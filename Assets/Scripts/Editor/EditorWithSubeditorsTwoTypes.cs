using UnityEngine;
using UnityEditor;

public abstract class EditorWithSubEditorsTwoTypes<TEditor1, TTarget1, TEditor2, TTarget2> : Editor
    where TEditor1 : Editor
    where TTarget1 : Object
    where TEditor2 : Editor
    where TTarget2 : Object
{
    protected TEditor1[] subEditors1;
    protected TEditor2[] subEditors2;

    public void CheckAndCreateSubEditors(TTarget1[] subEditorTargets1, TTarget2[] subEditorTargets2)
    {
        CheckAndCreateSubEditors(subEditorTargets1);
        CheckAndCreateSubEditors(subEditorTargets2);
    }

    public void CleanupEditors()
    {
        CleanupEditors1();
        CleanupEditors2();
    }

    private void CheckAndCreateSubEditors(TTarget1[] subEditorTargets)
    {
        if (subEditors1 != null && subEditors1.Length == subEditorTargets.Length)
        {
            return;
        }

        CleanupEditors1();

        subEditors1 = new TEditor1[subEditorTargets.Length];

        for (int i = 0; i < subEditors1.Length; i++)
        {
            subEditors1[i] = CreateEditor(subEditorTargets[i] as TTarget1) as TEditor1;
            SubEditorSetup(subEditors1[i]);
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

    private void CleanupEditors1()
    {
        if (subEditors1 == null)
            return;

        for (int i = 0; i < subEditors1.Length; i++)
        {
            DestroyImmediate(subEditors1[i]);
        }

        subEditors1 = null;
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