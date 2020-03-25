using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorUtils
{
    public static void DrawHorizontalLine(Color color, int thickness = 2, int paddingV = 10, int paddingH = 0)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(paddingV + thickness));
        r.height = thickness;
        r.y += paddingV / 2;
        r.x += (-2 + paddingH);
        r.width -= paddingH;
        EditorGUI.DrawRect(r, color);
    }
}
