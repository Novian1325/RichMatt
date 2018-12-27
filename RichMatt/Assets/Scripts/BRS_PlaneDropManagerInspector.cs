using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(BRS_PlaneDropManager))]
public class BRS_PlaneDropManagerInspector : Editor
{
    public Texture2D splashTexture;

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(100f));
        GUI.DrawTexture(rect, splashTexture, ScaleMode.ScaleToFit, true, 0f);
        base.OnInspectorGUI();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

       // GUILayout.Label("this is a test, this is only a test");
    }
}