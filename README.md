# Unity Spline Editor
[![Unity 2019.4+](https://img.shields.io/badge/unity-2019.4%2B-blue.svg)](https://unity3d.com/get-unity/download) [![openupm](https://img.shields.io/npm/v/com.vvrvvd.spline-editor?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.vvrvvd.spline-editor/) [![license badge](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

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
- Casting splines to either self, custom transform or camera using Physics Raycast
- Casting control points to current mouse position using Physics Raycast
- Drawer tool for drawing splines in Scene View
- Three buttons layouts types: Image, Text and Image & Text
- Adjustable editor settings (colors, sizes etc.)
- Adjustable shortcuts through Unity Shortcuts manager
- Example scripts showing how the tool may be used along with Line Renderer

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

Editor window can be opened by going to `Window->Spline Editor` on the Unity toolbar. Spline can be edited by **adding**, **removing** or **splitting** curves. Additionaly every control point has three **modes** that can influence a neighbour control point. When spline is **looped** the first and the last points are treated as the same point so they share the same position and mode (keep in mind that they are still two distinct points). Splines can also be quickly **factored** or **simplified** by adding or removing mid points from them.

 <img src="https://i.imgur.com/o3CVT8e.gif">
 <img src="https://i.imgur.com/uVQE4iX.gif">
 <img src="https://i.imgur.com/AfDWVpm.gif">

### Casting Spline <a name="casting-spline"></a>

Splines can be casted to **custom transform**, **self** (when custom transform is set to null) or to current **scene camera view**. You can also cast selected control points to mouse position using a shortuct. The casting is implemented with Physics Raycast so it works only with colliders. 

 <img src="https://i.imgur.com/6DTlYlx.gif">
 <img src="https://i.imgur.com/cpnBNMR.gif">
 <img src="https://i.imgur.com/aYKMNxq.gif">

### Drawer Tool <a name="drawer-tool"></a>

**Drawer tool** lets you freely draw spline in the scene view. Use additional parameters like **smoothing acute angles** or **segment length** to adjust the tool to your needs.

 <img src="https://i.imgur.com/4jYvQq6.gif">

### Settings <a name="settings"></a>

**Settings** can be accessed by going to `Edit->Project Settings->Spline Editor` on the Unity toolbar or by clicking on a gear icon in the right corner of Spline Editor window.

 <img src="https://i.imgur.com/GZn0Hin.gif">
 
 ## License <a name="license"></a>
 
[MIT](LICENSE)
