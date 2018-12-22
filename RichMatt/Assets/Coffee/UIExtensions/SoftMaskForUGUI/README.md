SoftMaskForUGUI
===

Soft masking for uGUI elements in Unity.

![](https://user-images.githubusercontent.com/12690315/48702871-6a8f5400-ec35-11e8-8940-8d6fbb1de688.png)

[![](https://img.shields.io/github/release/mob-sakai/SoftMaskForUGUI.svg?label=latest%20version)](https://github.com/mob-sakai/SoftMaskForUGUI/releases)
[![](https://img.shields.io/github/release-date/mob-sakai/SoftMaskForUGUI.svg)](https://github.com/mob-sakai/SoftMaskForUGUI/releases)
![](https://img.shields.io/badge/unity-2017%2B-green.svg)
[![](https://img.shields.io/github/license/mob-sakai/SoftMaskForUGUI.svg)](https://github.com/mob-sakai/SoftMaskForUGUI/blob/master/LICENSE.txt)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-orange.svg)](http://makeapullrequest.com)

<< [Description](#Description) | [WebGL Demo](#demo) | [Download](https://github.com/mob-sakai/SoftMaskForUGUI/releases) | [Usage](#usage) | [Development Note](#development-note) >>

### What's new? [See changelog ![](https://img.shields.io/github/release-date/mob-sakai/SoftMaskForUGUI.svg?label=last%20updated)](https://github.com/mob-sakai/SoftMaskForUGUI/blob/develop/CHANGELOG.md)
### Do you want to receive notifications for new releases? [Watch this repo ![](https://img.shields.io/github/watchers/mob-sakai/SoftMaskForUGUI.svg?style=social&label=Watch)](https://github.com/mob-sakai/SoftMaskForUGUI/subscription)



<br><br><br><br>
## Description

SoftMask is a smooth masking component for uGUI elements in Unity.
By using SoftMask instead of default Mask, rounded edges of UI elements can be expressed beautifully.

![](https://user-images.githubusercontent.com/12690315/48704344-a0363c00-ec39-11e8-87e5-b628b2a89923.png)

#### Features

* SoftMask is compatible with Mask.
* You can adjust the visible part.  
![](https://user-images.githubusercontent.com/12690315/48661087-01ca9f00-eab0-11e8-8878-772a1ed1fb7b.gif)
* Text, Image, RawImage can be used as a masking.
* Support multiple-sprites and SpriteAtlas.
* Support up to 4 nested soft masks.  
![](https://user-images.githubusercontent.com/12690315/48708326-a0d4cf80-ec45-11e8-83b8-f55d29138db7.png)
* Support scroll view.  
![](https://user-images.githubusercontent.com/12690315/48708527-2b1d3380-ec46-11e8-9adf-9d33498f0353.png)
* Support inversed soft mask.  
![](https://user-images.githubusercontent.com/12690315/48708328-a0d4cf80-ec45-11e8-9945-e877faabc968.png)
* Support overlay, camera space and world space.  
![](https://user-images.githubusercontent.com/12690315/48708329-a0d4cf80-ec45-11e8-8328-16b697f981ec.png)
* Raycast is filtered only for the visible part.  
![](https://user-images.githubusercontent.com/12690315/48708330-a16d6600-ec45-11e8-94bf-afecd1bd9a39.png)
* Contain soft maskable UI shader.
* Support soft masks in your custom shaders by adding just 3 lines. For details, please see [Development Note](#support-soft-masks-in-your-custom-shaders). 
* Adjust soft mask buffer size to improve performance.
* Convert existing Mask to SoftMask from context menu.  
![](https://user-images.githubusercontent.com/12690315/48659018-902e2900-ea8e-11e8-9b6e-224365cdde7f.png)


#### Future plans

* Render the soft mask buffer only when needed to improve performance.
* Add a SoftMaskable component to the child UI elements of SoftMask From the inspector.
* Preview soft mask buffer in inspector.
* Support TextMeshPro.
* Component icon.

#### Known issues

* SceneView does not display SoftMask properly. It is displayed like Mask. ([By design](#why-is-not-it-displayed-properly-in-sceneview?))

#### Components

|Component|Description|Screenshot|
|-|-|-|
|SoftMask|Use instead of Mask for smooth masking.<br><br>**Show Mask Graphic:** Show the graphic that is associated with the Mask render area.<br>**Desampling Rate:** The desampling rate for soft mask buffer. The larger the value, the better the performance but the lower the quality.<br>**Softness:** The value used by the soft mask to select the area of influence defined over the soft mask's graphic.<br>**Ignore Parent:** Should the soft mask ignore parent soft masks?|<img src="https://user-images.githubusercontent.com/12690315/48741492-b1656400-ec9e-11e8-8c15-e1dbc41e5a20.png" width="600px">|
|SoftMaskable|Add this component to Graphic under SoftMask for smooth masking.<br><br>**Inverse:** The graphic will be visible only in areas where no mask is present.|<img src="https://user-images.githubusercontent.com/12690315/48741494-b1fdfa80-ec9e-11e8-9f79-8e88ebbeff45.png" width="600px">|



<br><br><br><br>
## Demo

[WebGL Demo](http://mob-sakai.github.io/SoftMaskForUGUI)



<br><br><br><br>
## Usage

1. Download `*.unitypackage` from [Releases](https://github.com/mob-sakai/SoftMaskForUGUI/releases).
2. Import the package into your Unity project. Select `Import Package > Custom Package` from the `Assets` menu.  
![](https://user-images.githubusercontent.com/12690315/46570979-edbb5a00-c9a7-11e8-845d-c5ee279effec.png)
3. Add SoftMask component instead of Mask component. Or, convert existing Mask component to SoftMask component from the context menu.  
![](https://user-images.githubusercontent.com/12690315/48659018-902e2900-ea8e-11e8-9b6e-224365cdde7f.png)
4. Add SoftMaskable components to the child UI elements of SoftMask component.  
![](https://user-images.githubusercontent.com/12690315/48704424-d4a9f800-ec39-11e8-8d65-8b7d1975750c.png) 
5. Adjust softness of SoftMask.  
![](https://user-images.githubusercontent.com/12690315/48661087-01ca9f00-eab0-11e8-8878-772a1ed1fb7b.gif)
6. Enjoy!


##### Requirement

* Unity 2017+ *(including Unity 2018.x)* 
* No other SDK are required



<br><br><br><br>
## Development Note

#### Support soft masks in your custom shaders

You can support soft masks in your custom shaders, by adding just 3 lines!

1. Add `#pragma` and `#include`.  `SOFTMASK_EDITOR` is a keyword for editor, not included in the build.
```
#include "Assets/Coffee/UIExtensions/SoftMaskForUGUI/SoftMask.cginc"
#pragma shader_feature __ SOFTMASK_EDITOR
```
2. Apply a soft mask in the fragment shader. `IN.vertex` is clip position.
```
color.a *= SoftMask(IN.vertex);
```

For details, please see [UI-Default-SoftMask.shader](https://raw.githubusercontent.com/mob-sakai/SoftMaskForUGUI/master/Assets/Coffee/UIExtensions/SoftMaskForUGUI/Resources/UI-Default-SoftMask.shader).


<br><br>
#### Why is not it displayed properly in SceneView?

SoftMask calculates the final alpha value based on the value of each channel of the soft mask buffer.
The soft mask buffer is a buffer generated based on GameView's screen space.

Since SceneView has a view matrix and a projection matrix independent of GameView, the SceneView's camera can not refer to the soft mask buffer properly.

Therefore, in GameView, it is displayed properly. but in ScreenView, it is displayed like default Mask.

![](https://user-images.githubusercontent.com/12690315/48704647-80ebde80-ec3a-11e8-8b2a-99095af85442.png)


<br><br>
#### Tips: Convert component from context menu

Converting components from the context menu is very convenient.
You can convert multiple components at the same time, without having to remove the source components.

![](https://user-images.githubusercontent.com/12690315/48659018-902e2900-ea8e-11e8-9b6e-224365cdde7f.png)

If the destination component has the same properties as the source component, the value is set automatically.

In addition, if the destination component is compatible with the source component, it will not lose its reference.
For example, if `public Mask mask;` refers to a Mask, converting it to SoftMask in this way does not lose references.

This way consists of two generic methods.

```cs
/// <summary>
/// Verify whether it can be converted to the specified component.
/// </summary>
protected static bool CanConvertTo<T>(Object context)
	where T : MonoBehaviour
{
	return context && context.GetType() != typeof(T);
}

/// <summary>
/// Convert to the specified component.
/// </summary>
protected static void ConvertTo<T>(Object context) where T : MonoBehaviour
{
	var target = context as MonoBehaviour;
	var so = new SerializedObject(target);
	so.Update();

	bool oldEnable = target.enabled;
	target.enabled = false;

	// Find MonoScript of the specified component.
	foreach (var script in Resources.FindObjectsOfTypeAll<MonoScript>())
	{
		if (script.GetClass() != typeof(T))
			continue;

		// Set 'm_Script' to convert.
		so.FindProperty("m_Script").objectReferenceValue = script;
		so.ApplyModifiedProperties();
		break;
	}

	(so.targetObject as MonoBehaviour).enabled = oldEnable;
}
```

In SoftMask, they are implemented as follows.
* Mask and SoftMask can be converted to each other.
* If conversion is not possible, gray out the context menu.

```cs
[MenuItem("CONTEXT/Mask/Convert To SoftMask", true)]
static bool _ConvertToSoftMask(MenuCommand command)
{
	return CanConvertTo<SoftMask>(command.context);
}
[MenuItem("CONTEXT/Mask/Convert To SoftMask", false)]
static void ConvertToSoftMask(MenuCommand command)
{
	ConvertTo<SoftMask>(command.context);
}
[MenuItem("CONTEXT/Mask/Convert To Mask", true)]
static bool _ConvertToMask(MenuCommand command)
{
	return CanConvertTo<Mask>(command.context);
}
[MenuItem("CONTEXT/Mask/Convert To Mask", false)]
static void ConvertToMask(MenuCommand command)
{
	ConvertTo<Mask>(command.context);
}
```

For details, please see [SoftMaskEditor.cs](https://raw.githubusercontent.com/mob-sakai/SoftMaskForUGUI/master/Assets/Coffee/UIExtensions/SoftMaskForUGUI/Scripts/Editor/SoftMaskEditor.cs).


<br><br>
#### Tips: Shader code for editor only

Do you know how to implement shader code "for editor only"?
SoftMask uses `SOFTMASK_EDITOR` keyword in shader code to determine whether it is running in the editor.

`#pragma shader_feature __ SOFTMASK_EDITOR`

`SOFTMASK_EDITOR` keyword is set from the editor script, but it is not set after it is built. Also, this shader variant will be excluded from build.

```cs
#if UNITY_EDITOR
material = new Material(shader);
material.hideFlags = HideFlags.HideAndDontSave;
material.EnableKeyword("SOFTMASK_EDITOR");
#endif
```


<br><br>
#### Tips: Shader code for SceneView only

Do you know how to implement shader code "for SceneView only"?
SoftMask understands that the current rendering is for SceneView, when SceneView's view projection matrix and UNITY_MATRIX_VP match.

`fixed isSceneView = 1 - any(UNITY_MATRIX_VP - _SceneViewVP);`

Actually, because of the movement operation in SceneView, use "approximate" instead of "match".

```cs
float4x4 _SceneViewVP;

fixed Approximate(float4x4 a, float4x4 b)
{
	float4x4 d = abs(a - b);
	return step(
		max(d._m00,max(d._m01,max(d._m02,max(d._m03,
		max(d._m10,max(d._m11,max(d._m12,max(d._m13,
		max(d._m20,max(d._m21,max(d._m22,max(d._m23,
		max(d._m30,max(d._m31,max(d._m32,d._m33))))))))))))))),
		0.01);
}

fixed isSceneView = Approximate(UNITY_MATRIX_VP, _SceneViewVP);
```

`_SceneViewVP` is set every frame from the editor script.

```cs
#if UNITY_EDITOR
UnityEditor.EditorApplication.update += ()
{
    Camera cam = UnityEditor.SceneView.lastActiveSceneView.camera;
    Matrix4x4 vp = cam.projectionMatrix * cam.worldToCameraMatrix;
    material.SetMatrix("_SceneViewVP", vp);
};
#endif
```



<br><br><br><br>
## License

* MIT
* © UTJ/UCL



## Author

[mob-sakai](https://github.com/mob-sakai)



## See Also

* GitHub page : https://github.com/mob-sakai/SoftMaskForUGUI
* Releases : https://github.com/mob-sakai/SoftMaskForUGUI/releases
* Issue tracker : https://github.com/mob-sakai/SoftMaskForUGUI/issues
* Current project : https://github.com/mob-sakai/SoftMaskForUGUI/projects/1
* Change log : https://github.com/mob-sakai/SoftMaskForUGUI/blob/master/CHANGELOG.md
