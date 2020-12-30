using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//This class is used to store some utility functions for use in the editor windows
public static class EditorUtility
{
    //Draw a horizontal line - good for dividing up sections of the window
    //https://forum.unity.com/threads/horizontal-line-in-editor-window.520812/ 
    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }
}
