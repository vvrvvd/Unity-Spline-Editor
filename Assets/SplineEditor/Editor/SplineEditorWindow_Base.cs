// <copyright file="SplineEditorWindow_Base.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing custom editor window to SplineEditor.
	/// </summary>
	public partial class SplineEditorWindow : EditorWindow
	{
		private int buttonsLayoutIndex = 2;

		private bool repaintScene = false;
		private bool initializeStyles = false;

		private Vector2 scrollPos = Vector2.zero;

		private static SplineEditorState EditorState => SplineEditorState.instance;

		private static SplineEditorWindowState EditorWindowState => SplineEditorWindowState.instance;

		private static SplineEditorConfiguration EditorSettings => SplineEditorConfiguration.Instance;

		private bool IsCurveEditorEnabled => IsSplineEditorEnabled && EditorState.IsAnyPointSelected;

		private bool IsSplineEditorEnabled => EditorState.CurrentSpline != null;

		/// <summary>
		/// Creates new SplineEditor Window if it's not found in the Unity view, otherwise just shows it.
		/// </summary>
		[MenuItem("Window/Spline Editor")]
		public static void Initialize()
		{
			var inspectorType = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
			var window = GetWindow<SplineEditorWindow>(WindowTitle, false, inspectorType);
			window.initializeStyles = true;
			window.autoRepaintOnSceneChange = true;
			window.Show();
		}

		private void OnEnable()
		{
			EditorState.OnSplineModified += OnSplineModified;
			EditorState.OnSelectedSplineChanged += OnSelectedSplineChanged;
			EditorState.OnSelectedPointChanged += OnSelectedCurveChanged;
		}

		private void OnDisable()
		{
			EditorState.OnSplineModified -= OnSplineModified;
			EditorState.OnSelectedSplineChanged -= OnSelectedSplineChanged;
			EditorState.OnSelectedPointChanged -= OnSelectedCurveChanged;
		}

		private void OnSplineModified()
		{
			EditorState.IsDrawerMode = EditorState.CurrentEditor != null && EditorState.IsDrawerMode;
			EditorState.IsNormalsEditorMode = EditorState.CurrentEditor != null && EditorState.IsNormalsEditorMode;
			Repaint();
		}

		private void OnSelectedSplineChanged()
		{
			EditorState.IsDrawerMode = EditorState.CurrentEditor != null && EditorState.IsDrawerMode;
			EditorState.IsNormalsEditorMode = EditorState.CurrentEditor != null && EditorState.IsNormalsEditorMode;
			OnSelectedCurveChanged();
		}

		private void OnSelectedCurveChanged()
		{
			Repaint();
		}

		private void OnGUI()
		{
			repaintScene = false;

			// Hack for getting hover mouse visuals before showing tooltip when using custom GUI.skin pt.1
			wantsMouseMove = true;

			if (initializeStyles)
			{
				InitializeStyles();
				initializeStyles = false;
			}

			EditorState.UpdateSplineStates();

			scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
			EditorGUILayout.BeginVertical();
			GUILayout.Space(10);
			DrawHeader();
			GUILayout.Space(10);
			DrawSettingsButton();
			DrawLayoutsToolbar();

			UpdateStyles();

			DrawPointGroup();
			GUILayout.Space(3);
			DrawCurveOptions();
			GUILayout.Space(3);
			DrawSplineGroup();
			GUILayout.Space(3);
			DrawNormalsEditorOptions();
			GUILayout.Space(3);
			DrawDrawerToolOptions();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();

			// Hack for getting hover mouse visuals before showing tooltip when using custom GUI.skin pt.2
			if (Event.current.type == EventType.MouseMove)
			{
				Repaint();
			}

			if (repaintScene)
			{
				SceneView.RepaintAll();
			}
		}

		private void DrawHeader()
		{
			var headerContent = new GUIContent(HeaderTitle);
			GUILayout.Label(headerContent, headerLabelStyle);
		}

		private void DrawLayoutsToolbar()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			buttonsLayoutIndex = GUILayout.Toolbar(buttonsLayoutIndex, layoutsButtonsContent, ToolsHeaderToolbarWidth, ToolsHeaderToolbarHeight);

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawSettingsButton()
		{
			GUILayout.Space(-40);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button(settingsButtonContent, settingsButtonStyle, ToolsSettingsButtonWidth, ToolsSettingsButtonHeight))
			{
				SettingsService.OpenProjectSettings("Project/Spline Editor");
			}

			GUILayout.EndHorizontal();
		}
	}
}
