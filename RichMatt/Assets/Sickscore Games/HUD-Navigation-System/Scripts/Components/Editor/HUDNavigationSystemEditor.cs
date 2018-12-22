using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using SickscoreGames.HUDNavigationSystem;

[CustomEditor(typeof(HUDNavigationSystem))]
public class HUDNavigationSystemEditor : HUDNavigationBaseEditor
{
	#region Variables
	protected HUDNavigationSystem hudTarget;
	private bool _radar_, _compassBar_, _indicator_, _minimap_;
	#endregion


	#region Main Methods
	void OnEnable ()
	{
		editorTitle = "HUD Navigation System";
		splashTexture = (Texture2D)Resources.Load ("Textures/splashTexture_System", typeof(Texture2D));

		hudTarget = (HUDNavigationSystem)target;
	}


	protected override void OnBaseInspectorGUI ()
	{
		// update serialized object
		serializedObject.Update ();

		// cache serialized properties
		SerializedProperty _pPlayerCamera = serializedObject.FindProperty ("PlayerCamera");
		SerializedProperty _pPlayerController = serializedObject.FindProperty ("PlayerController");
		SerializedProperty _pRotationReference = serializedObject.FindProperty ("RotationReference");

		SerializedProperty _pUseRadar = serializedObject.FindProperty ("useRadar");
		SerializedProperty _pRadarMode = serializedObject.FindProperty ("radarMode");
		SerializedProperty _pRadarZoom = serializedObject.FindProperty ("radarZoom");
		SerializedProperty _pRadarRadius = serializedObject.FindProperty ("radarRadius");
		SerializedProperty _pRadarMaxRadius = serializedObject.FindProperty ("radarMaxRadius");
		SerializedProperty _pUseRadarHeightSystem = serializedObject.FindProperty ("useRadarHeightSystem");
		SerializedProperty _pRadarDistanceAbove = serializedObject.FindProperty ("radarDistanceAbove");
		SerializedProperty _pRadarDistanceBelow = serializedObject.FindProperty ("radarDistanceBelow");
		SerializedProperty _pShowRadarHeightGizmos = serializedObject.FindProperty ("showRadarHeightGizmos");
		SerializedProperty _pRadarHeightGizmoSize = serializedObject.FindProperty ("radarHeightGizmoSize");
		SerializedProperty _pRadarHeightGizmoColor = serializedObject.FindProperty ("radarHeightGizmoColor");

		SerializedProperty _pUseCompassBar = serializedObject.FindProperty ("useCompassBar");
		SerializedProperty _pCompassBarRadius = serializedObject.FindProperty ("compassBarRadius");

		SerializedProperty _pUseIndicators = serializedObject.FindProperty ("useIndicators");
		SerializedProperty _pIndicatorRadius = serializedObject.FindProperty ("indicatorRadius");
		SerializedProperty _pIndicatorHideDistance = serializedObject.FindProperty ("indicatorHideDistance");
		SerializedProperty _pUseOffscreenIndicators = serializedObject.FindProperty ("useOffscreenIndicators");
		SerializedProperty _pIndicatorOffscreenBorder = serializedObject.FindProperty ("indicatorOffscreenBorder");
		SerializedProperty _pUseIndicatorScaling = serializedObject.FindProperty ("useIndicatorScaling");
		SerializedProperty _pIndicatorScaleRadius = serializedObject.FindProperty ("indicatorScaleRadius");
		SerializedProperty _pIndicatorMinScale = serializedObject.FindProperty ("indicatorMinScale");

		SerializedProperty _pUseMinimap = serializedObject.FindProperty ("useMinimap");
		SerializedProperty _pMinimapProfile = serializedObject.FindProperty ("minimapProfile");
		SerializedProperty _pMinimapMode = serializedObject.FindProperty ("minimapMode");
		SerializedProperty _pMinimapScale = serializedObject.FindProperty ("minimapScale");
		SerializedProperty _pMinimapRadius = serializedObject.FindProperty ("minimapRadius");
		SerializedProperty _pShowMinimapBounds = serializedObject.FindProperty ("showMinimapBounds");
		SerializedProperty _pMinimapBoundsGizmoColor = serializedObject.FindProperty ("minimapBoundsGizmoColor");
		SerializedProperty _pUseMinimapHeightSystem = serializedObject.FindProperty ("useMinimapHeightSystem");
		SerializedProperty _pMinimapDistanceAbove = serializedObject.FindProperty ("minimapDistanceAbove");
		SerializedProperty _pMinimapDistanceBelow = serializedObject.FindProperty ("minimapDistanceBelow");
		SerializedProperty _pShowMinimapHeightGizmos = serializedObject.FindProperty ("showMinimapHeightGizmos");
		SerializedProperty _pMinimapHeightGizmoSize = serializedObject.FindProperty ("minimapHeightGizmoSize");
		SerializedProperty _pMinimapHeightGizmoColor = serializedObject.FindProperty ("minimapHeightGizmoColor");

		// REFERENCES
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (_pPlayerCamera);
		EditorGUILayout.PropertyField (_pPlayerController);
		EditorGUILayout.PropertyField (_pRotationReference, new GUIContent ("Rotation Reference", "The transform you want to use as rotation reference."));
		EditorGUILayout.EndVertical ();

		// FEATURES
		GUILayout.Space (8); // SPACE
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.PropertyField (_pUseRadar, new GUIContent ("Radar Feature"));
		EditorGUILayout.LabelField ((hudTarget.useRadar) ? "ENABLED" : "DISABLED", (hudTarget.useRadar) ? enabledStyle : disabledStyle, GUILayout.Width (100));
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.PropertyField (_pUseCompassBar, new GUIContent ("Compass Bar Feature"));
		EditorGUILayout.LabelField ((hudTarget.useCompassBar) ? "ENABLED" : "DISABLED", (hudTarget.useCompassBar) ? enabledStyle : disabledStyle, GUILayout.Width (100));
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.PropertyField (_pUseIndicators, new GUIContent ("Indicator Feature"));
		EditorGUILayout.LabelField ((hudTarget.useIndicators) ? "ENABLED" : "DISABLED", (hudTarget.useIndicators) ? enabledStyle : disabledStyle, GUILayout.Width (100));
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.PropertyField (_pUseMinimap, new GUIContent ("Minimap Feature"));
		EditorGUILayout.LabelField ((hudTarget.useMinimap) ? "ENABLED" : "DISABLED", (hudTarget.useMinimap) ? enabledStyle : disabledStyle, GUILayout.Width (100));
		EditorGUILayout.EndHorizontal ();
		GUILayout.Space (8); // SPACE

		// RADAR SETTINGS
		if (hudTarget.useRadar) {
			EditorGUILayout.BeginVertical (boxStyle);
			_radar_ = EditorGUILayout.BeginToggleGroup ("Radar Settings", _radar_);
			if (_radar_) {
				GUILayout.Space (4); // SPACE
				// CONTENT BEGIN
				EditorGUILayout.PropertyField (_pRadarMode);
				EditorGUILayout.Slider (_pRadarZoom, .1f, 5f, "Radar Zoom");
				EditorGUILayout.Slider (_pRadarRadius, 1f, 500f, "Radar Radius");
				EditorGUILayout.Slider (_pRadarMaxRadius, 1f, 500f, "Radar Radius (Border)");
				if (_pRadarMaxRadius.floatValue < _pRadarRadius.floatValue)
					_pRadarMaxRadius.floatValue = _pRadarRadius.floatValue;

				// height system settings
				GUILayout.Space (4); // SPACE
				EditorGUILayout.BeginVertical (boxStyle);
				_pUseRadarHeightSystem.boolValue = EditorGUILayout.ToggleLeft ("Enable Height System", _pUseRadarHeightSystem.boolValue, subHeaderStyle);
				if (hudTarget.useRadarHeightSystem) {
					GUILayout.Space (4); // SPACE
					EditorGUILayout.Slider (_pRadarDistanceAbove, 1f, 100f, new GUIContent ("Min. Distance Above"));
					EditorGUILayout.Slider (_pRadarDistanceBelow, 1f, 100f, new GUIContent ("Min. Distance Below"));
					EditorGUILayout.PropertyField (_pShowRadarHeightGizmos, new GUIContent ("Show Debug Gizmos"));
					if (hudTarget.showRadarHeightGizmos) {
						EditorGUILayout.PropertyField (_pRadarHeightGizmoSize, new GUIContent ("> Gizmo Size"));
						EditorGUILayout.PropertyField (_pRadarHeightGizmoColor, new GUIContent ("> Gizmo Color"));
					}
				}
				EditorGUILayout.EndVertical ();
				// CONTENT ENDOF
			}
			EditorGUILayout.EndToggleGroup ();
			EditorGUILayout.EndVertical ();
		}

		// COMPASS BAR SETTINGS
		if (hudTarget.useCompassBar) {
			EditorGUILayout.BeginVertical (boxStyle);
			_compassBar_ = EditorGUILayout.BeginToggleGroup ("Compass Bar Settings", _compassBar_);
			if (_compassBar_) {
				GUILayout.Space (4); // SPACE
				// CONTENT BEGIN
				EditorGUILayout.Slider (_pCompassBarRadius, 1f, 500f, "Compass Bar Radius");
				// CONTENT ENDOF
			}
			EditorGUILayout.EndToggleGroup ();
			EditorGUILayout.EndVertical ();
		}

		// INDICATOR SETTINGS
		if (hudTarget.useIndicators) {
			EditorGUILayout.BeginVertical (boxStyle);
			_indicator_ = EditorGUILayout.BeginToggleGroup ("Indicator Settings", _indicator_);
			if (_indicator_) {
				GUILayout.Space (4); // SPACE
				// CONTENT BEGIN
				EditorGUILayout.Slider (_pIndicatorRadius, 1f, 500f, "Indicator Radius");
				EditorGUILayout.Slider (_pIndicatorHideDistance, 0f, 50f, "Indicator Hide Distance");

				// off-screen indicator settings
				GUILayout.Space (4); // SPACE
				EditorGUILayout.BeginVertical (boxStyle);
				_pUseOffscreenIndicators.boolValue = EditorGUILayout.ToggleLeft ("Enable Offscreen Indicators", _pUseOffscreenIndicators.boolValue, subHeaderStyle);
				if (hudTarget.useOffscreenIndicators) {
					GUILayout.Space (4); // SPACE
					EditorGUILayout.Slider (_pIndicatorOffscreenBorder, 0f, 1f, "Screen Border");
				}
				EditorGUILayout.EndVertical ();

				// indicator scaling settings
				GUILayout.Space (4); // SPACE
				EditorGUILayout.BeginVertical (boxStyle);
				_pUseIndicatorScaling.boolValue = EditorGUILayout.ToggleLeft ("Enable Distance Scaling", _pUseIndicatorScaling.boolValue, subHeaderStyle);
				if (hudTarget.useIndicatorScaling) {
					GUILayout.Space (4); // SPACE
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.Slider (_pIndicatorScaleRadius, 1f, 500f, "Scale Radius");
					if (hudTarget.indicatorScaleRadius > hudTarget.indicatorRadius)
						hudTarget.indicatorScaleRadius = hudTarget.indicatorRadius;
					EditorGUILayout.Slider (_pIndicatorMinScale, .1f, 1f, "Minimum Scale");
					if (showHelpboxes)
						EditorGUILayout.HelpBox ("Indicator will be scaled by distance within the defined radius.", MessageType.Info);
					EditorGUILayout.EndVertical ();
				}
				EditorGUILayout.EndVertical ();
				// CONTENT ENDOF
			}
			EditorGUILayout.EndToggleGroup ();
			EditorGUILayout.EndVertical ();
		}

		// MINIMAP SETTINGS
		if (hudTarget.useMinimap) {
			EditorGUILayout.BeginVertical (boxStyle);
			_minimap_ = EditorGUILayout.BeginToggleGroup ("Minimap Settings", _minimap_);
			if (_minimap_) {
				GUILayout.Space (4); // SPACE
				// CONTENT BEGIN
				EditorGUILayout.PropertyField (_pMinimapProfile, new GUIContent ("Minimap Profile"));
				if (hudTarget.minimapProfile != null) {
					EditorGUILayout.PropertyField (_pMinimapMode);
					EditorGUILayout.Slider (_pMinimapScale, .01f, 1f, "Minimap Scale");
					EditorGUILayout.Slider (_pMinimapRadius, 1f, 500f, "Minimap Radius");
					EditorGUILayout.PropertyField (_pShowMinimapBounds, new GUIContent ("Show Minimap Bounds"));
					if (hudTarget.showMinimapBounds)
						EditorGUILayout.PropertyField (_pMinimapBoundsGizmoColor, new GUIContent ("> Gizmo Color"));

					// height system settings
					GUILayout.Space (4); // SPACE
					EditorGUILayout.BeginVertical (boxStyle);
					_pUseMinimapHeightSystem.boolValue = EditorGUILayout.ToggleLeft ("Enable Height System", _pUseMinimapHeightSystem.boolValue, subHeaderStyle);
					if (hudTarget.useMinimapHeightSystem) {
						GUILayout.Space (4); // SPACE
						EditorGUILayout.Slider (_pMinimapDistanceAbove, 1f, 100f, new GUIContent ("Min. Distance Above"));
						EditorGUILayout.Slider (_pMinimapDistanceBelow, 1f, 100f, new GUIContent ("Min. Distance Below"));
						EditorGUILayout.PropertyField (_pShowMinimapHeightGizmos, new GUIContent ("Show Debug Gizmos"));
						if (hudTarget.showMinimapHeightGizmos) {
							EditorGUILayout.PropertyField (_pMinimapHeightGizmoSize, new GUIContent ("> Gizmo Size"));
							EditorGUILayout.PropertyField (_pMinimapHeightGizmoColor, new GUIContent ("> Gizmo Color"));
						}
					}
					EditorGUILayout.EndVertical ();
				}

				GUILayout.Space (4); // SPACE

				if (hudTarget.minimapProfile == null) {
					// create profile button
					if (GUILayout.Button ("Create New Profile", GUILayout.Height (20))) {
						GameObject textureCreatorGO = new GameObject ("HNS TextureCreator");
						textureCreatorGO.transform.position = Vector3.zero;
						textureCreatorGO.AddComponent<HNSTextureCreator> ();
						Selection.activeGameObject = textureCreatorGO;
					}
				}
				// CONTENT ENDOF
			}
			EditorGUILayout.EndToggleGroup ();
			EditorGUILayout.EndVertical ();
		}

		// show/hide expand button
		showExpandButton = hudTarget.useRadar || hudTarget.useCompassBar || hudTarget.useIndicators;

		// apply modified properties
		serializedObject.ApplyModifiedProperties ();
	}


	protected override void OnExpandSettings (bool value)
	{
		base.OnExpandSettings (value);
		_radar_ = _compassBar_ = _indicator_ = _minimap_ = value;
	}
	#endregion
}
