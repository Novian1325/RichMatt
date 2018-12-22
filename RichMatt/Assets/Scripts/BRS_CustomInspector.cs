using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BRS_Example))]
public class BRS_CustomInspector : Editor
{
    public Texture2D splashTexture;

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(100f));
        GUI.DrawTexture(rect, splashTexture, ScaleMode.ScaleAndCrop, true, 0f);
        base.OnInspectorGUI();
    }
}