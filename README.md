# Unity Spline Editor
[![Unity 2019.4+](https://img.shields.io/badge/unity-2019.4%2B-blue.svg)](https://unity3d.com/get-unity/download) [![openupm](https://img.shields.io/npm/v/com.vvrvvd.spline-editor?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.vvrvvd.spline-editor/) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Table Of Contents

- [Introduction](#introduction)
- [Features](#features)
- [System Requirements](#system-requirements)
- [Installation](#installation)
- [Overview](#overview)
	- [Spline Editor Window](#spline-editor-window)
	- [Casting Spline](#casting-spline)
	- [Drawer Tool](#drawer-tool)
	- [Settings](#settings)
	- [Examples](#examples)
		- [Mesh Generator](#mesh-spline)
		- [Line Renderer](#line-renderer)
- [License](#license)

## Introduction <a name="introduction"></a>

**Unity Spline Editor** is an open-source tool for creating and managing cubic bezier curves in Unity Editor. 

The tool was written to bring cubic bezier splines to Unity. The tool let you do the basic operations like **adding**, **removing** or **splitting** curves but also provides you with options to **factor**, **simplify**, **draw** or **cast splines to camera view**. You can also adjust how splines are displayed in the editor through settings window. 

## Features <a name="features"></a>

- Separate tools window for Spline Editor
- Creating and managing cubic bezier splines in Scene View
- Four modes for control points: **Free**, **Aligned**, **Mirrored** and **Automatic**
- Splitting curves by split point value
- Quick factoring and simplifying splines
- Normals editor
- Casting splines to either self, custom transform or camera using Physics Raycast
- Casting control points to current mouse position using Physics Raycast
- Drawer tool for drawing splines in Scene View
- Three buttons layouts types: Image, Text and Image & Text
- Adjustable editor settings (colors, sizes etc.)
- Adjustable shortcuts through Unity Shortcuts manager
- Example scripts for generating meshes from splines and using line renderers

## System Requirements <a name="system-requirements"></a>

Unity 2019.4 or newer.

## Installation <a name="installation"></a>

1. The package is available in Unity Package Manager via git URL. Follow up [this](https://docs.unity3d.com/Manual/upm-ui-giturl.html) Unity page for detailed instructions. Git URL:
```
https://github.com/vvrvvd/Unity-Spline-Editor.git#upm
```
2. You can also install Spline Editor by simply downloading repository zip file and copying Assets folder content to your Unity project.

## Overview <a name="overview"></a>

### Spline Editor Window <a name="spline-editor-window"></a>

Editor window can be opened by going to `Window->Spline Editor` on the Unity toolbar. Spline can be edited by **adding**, **removing** or **splitting** curves. Additionaly every control point has four **modes** that can influence a neighbour control point. When spline is **looped** the first and the last points are treated as the same point so they share the same position and mode (keep in mind that they are still two distinct points). Splines can also be quickly **factored** or **simplified** by adding or removing mid points from them.

 <img src="https://i.imgur.com/4gnWIvc.gif">
 <img src="https://i.imgur.com/I5uwhxj.gif">

### Normals Editor <a name="casting-spline"></a>

Spline points have normals that can be rotated along spline tangents. There is an option to **flip** normals or rotate all the normals by **global angle**.

 <img src="https://i.imgur.com/OnkY3z2.gif">

### Drawer Tool <a name="drawer-tool"></a>

**Drawer tool** lets you freely draw spline in the scene view. Use additional parameters like **smoothing acute angles** or **segment length** to adjust the tool to your needs.

 <img src="https://i.imgur.com/sSQlFne.gif">

### Settings <a name="settings"></a>

**Settings** can be accessed by going to `Edit->Project Settings->Spline Editor` on the Unity toolbar or by clicking on a gear icon in the right corner of Spline Editor window.

 <img src="https://i.imgur.com/kG2aeke.gif">
 
 
## Examples <a name="examples"></a>

### Mesh Generator <a name="mesh-spline"></a>
Add **Spline Mesh** component on scene to generate mesh from Bezier Spline.

It is located in `Samples/Mesh Generator` folder.

 <img src="https://i.imgur.com/gC0KoT8.gif">
 
### Line Renderer Spline <a name="line-renderer"></a>
To visualize spline using **Line Renderer**, add **Line Renderer Spline** component on scene.

It is located in `Samples/Line Renderer` folder.

 <img src="https://i.imgur.com/goQc1uk.gif">
 
 ## License <a name="license"></a>
 
[MIT](https://opensource.org/licenses/MIT)
