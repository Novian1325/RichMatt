using UnityEngine;
using UnityEditor;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    [CustomEditor(typeof(BRS_PlanePathManager))]
    public class BRS_PlanePathManagerInspector : Editor
    {
        public Texture2D splashTexture;

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(100f));
            GUI.DrawTexture(rect, splashTexture, ScaleMode.ScaleToFit, true, 0f);
            base.OnInspectorGUI();
        }
    }

}
