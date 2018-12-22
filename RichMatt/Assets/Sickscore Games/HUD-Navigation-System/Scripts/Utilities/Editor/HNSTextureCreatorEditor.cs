using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
#endif
using SickscoreGames.HUDNavigationSystem;

[CustomEditor(typeof(HNSTextureCreator))]
public class HNSTextureCreatorEditor : HUDNavigationBaseEditor
{
	#region Variables
	protected HNSTextureCreator hudTarget;
	private bool _fitToBounds_, _editorSettings_;
	private BoxBoundsHandle boxBoundsHandle = new BoxBoundsHandle ();
	#endregion


	#region Main Methods
	void OnEnable ()
	{
		editorTitle = "HNS Texture Creator";
		splashTexture = (Texture2D)Resources.Load ("Textures/splashTexture_TextureCreator", typeof(Texture2D));
		showExpandButton = false;

		hudTarget = (HNSTextureCreator)target;

		// hide default tools
		Tools.hidden = true;
	}


	void OnDisable ()
	{
		// restore default tools
		Tools.hidden = false;
	}


	protected override void OnBaseInspectorGUI ()
	{
		// update serialized object
		serializedObject.Update ();

		// cache serialized properties
		SerializedProperty _pMapName = serializedObject.FindProperty ("MapName");

		SerializedProperty _pTextureSize = serializedObject.FindProperty ("TextureSize");
		SerializedProperty _pBackground = serializedObject.FindProperty ("Background");

		SerializedProperty _pPreviewShowBounds = serializedObject.FindProperty ("PreviewShowBounds");
		SerializedProperty _pPreviewBoundsColor = serializedObject.FindProperty ("PreviewBoundsColor");

		SerializedProperty _pFitToObject = serializedObject.FindProperty ("FitToObject");
		SerializedProperty _pObjectBoundsMultiplier = serializedObject.FindProperty ("ObjectBoundsMultiplier");

		// MAP INFO BOX
		EditorGUILayout.BeginVertical (boxStyle);
		Vector3Int intMapBounds = Vector3Int.RoundToInt (hudTarget.MapBounds.size);
		EditorGUILayout.LabelField (new GUIContent (string.Format ("Map Bounds (XYZ): <b>{0} x {1} x {2}</b>", intMapBounds.x, intMapBounds.y, intMapBounds.z), "Current map bounds."), richLabelStyle);
		EditorGUILayout.EndVertical ();

		// TEXTURE INFO BOX
		EditorGUILayout.BeginVertical (boxStyle);
		Vector2Int textureSize = hudTarget.GetTotalTextureSize ();
		EditorGUILayout.LabelField (new GUIContent (string.Format ("Total Texture Size: <b>{0} x {1}</b>", textureSize.x, textureSize.y), "Total texture dimensions of the final texture."), richLabelStyle);
		if ((textureSize.x > 4096 || textureSize.y > 4096))
			EditorGUILayout.HelpBox ("A maximum size of 2048px or 4096px is recommended.\nOlder graphics cards may have issues with textures larger than 4096px!", MessageType.Warning);
		EditorGUILayout.EndVertical ();

		GUILayout.Space (4); // SPACE

		// enable/disable GUI
		bool guiEnabled = GUI.enabled;
		GUI.enabled = !hudTarget._isBusy;

		// SETTINGS
		_pMapName.stringValue = EditorGUILayout.TextField (new GUIContent ("Name", "Choose a name for the texture, that will be generated."), _pMapName.stringValue);
		_pMapName.stringValue = Regex.Replace (_pMapName.stringValue, @"[^A-Za-z0-9 _]+", "").Replace (" ", "_");
		int prevTextureSize = _pTextureSize.intValue;
		EditorGUILayout.PropertyField (_pTextureSize, new GUIContent ("Texture Size", "Total size of the final texture. (longest side)"));
		EditorGUILayout.PropertyField (_pBackground, new GUIContent ("Background", "Define a background color for areas where nothing will be rendered to the texture."));

		GUILayout.Space (4); // SPACE

		// EDITOR SETTINGS
		EditorGUILayout.BeginVertical (boxStyle);
		_editorSettings_ = EditorGUILayout.BeginToggleGroup ("Editor Settings", _editorSettings_);
		if (_editorSettings_) {
			GUILayout.Space (4); // SPACE
			EditorGUILayout.PropertyField (_pPreviewShowBounds, new GUIContent ("Show Map Bounds", "Toggle the visibility of the total map bounds."));
			if (_pPreviewShowBounds.boolValue)
				EditorGUILayout.PropertyField (_pPreviewBoundsColor, new GUIContent ("Map Bounds Color", "Customize the color of the bounding box."));
		}
		EditorGUILayout.EndToggleGroup ();
		EditorGUILayout.EndVertical ();

		// FIT TO BOUNDS
		EditorGUILayout.BeginVertical (boxStyle);
		_fitToBounds_ = EditorGUILayout.BeginToggleGroup ("Fit To Object Bounds", _fitToBounds_);
		if (_fitToBounds_) {
			GUILayout.Space (4); // SPACE
			EditorGUILayout.PropertyField (_pFitToObject, new GUIContent ("Object / Terrain", "Assign an object or terrain to fit the bounds to it's size."));
			EditorGUILayout.Slider (_pObjectBoundsMultiplier, .1f, 5f, new GUIContent ("Scale Multiplier", "Object's bounds (XZ) will be multiplied by this value.\n\n1 = default scale\n<1 = negative scale\n>1 = positive scale"));

			bool _guiEnabled = GUI.enabled;
			GUI.enabled = _guiEnabled && _pFitToObject.objectReferenceValue != null;
			if (GUILayout.Button ("Fit To Object Bounds", GUILayout.Height (20)))
				hudTarget.FitToBounds (hudTarget.FitToObject);
			GUI.enabled = _guiEnabled;
		}
		EditorGUILayout.EndToggleGroup ();
		EditorGUILayout.EndVertical ();

		GUILayout.Space (8); // SPACE

		// CREATE TEXTURE BUTTON
		if (GUILayout.Button ("CREATE TEXTURE", GUILayout.Height (30))) {
			if (!Mathf.IsPowerOfTwo (textureSize.x) || !Mathf.IsPowerOfTwo (textureSize.y)) {
				if (EditorUtility.DisplayDialog ("NPOT Texture Size", "Texture has NPOT (non power of two) dimensions, which can't be compressed by the engine. It's highly recommended to use POT (power of two) dimensions!\n\ne.g. 512x512, 1024x2048, 4096x2048, ...", "Ignore", "Abort"))
					hudTarget.CreateMapTexture ();
			} else {
				hudTarget.CreateMapTexture ();
			}
		}

		// re-enable GUI
		GUI.enabled = guiEnabled;

		// apply modified properties
		serializedObject.ApplyModifiedProperties ();
	}


	protected virtual void OnSceneGUI ()
	{
		// keep TextureCreator selected
		HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));

		// return if busy
		if (hudTarget._isBusy)
			return;
		
		// update bounds center with position handle
		EditorGUI.BeginChangeCheck ();
		Vector3 newCenter = Handles.PositionHandle (hudTarget.MapBounds.center, Quaternion.identity);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (hudTarget, "Move HNS MiniMap Bounds");
			hudTarget.MapBounds.center = newCenter;
		}

		// update bounds with box handles
		EditorGUI.BeginChangeCheck ();
		boxBoundsHandle.center = hudTarget.MapBounds.center;
		boxBoundsHandle.size = hudTarget.MapBounds.size;
		boxBoundsHandle.DrawHandle ();
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (hudTarget, "Change HNS MiniMap Bounds");
			hudTarget.MapBounds = new Bounds (boxBoundsHandle.center, boxBoundsHandle.size);
		}
	}


	void OnInspectorUpdate ()
	{
		Repaint ();
	}
	#endregion


	#region Utility Methods
	#endregion


	#region Subclasses
	#endregion
}
