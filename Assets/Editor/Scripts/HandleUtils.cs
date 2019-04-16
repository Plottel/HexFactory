using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class HandleUtils
{
    public static void CenteredLabel(Vector3 pos, string label)
    {
        var oldAlignment = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

        Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(label));
        pos.x -= (labelSize.x / 2 / 100); // Divide by 100 for PixelsToUnits
        pos.y -= (labelSize.y / 2 / 100); // Divide by 100 for PixelsToUnits

        Handles.Label(pos, label);

        GUI.skin.label.alignment = oldAlignment;
    }
}
