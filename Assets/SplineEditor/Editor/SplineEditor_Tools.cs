// <copyright file="SplineEditor_Tools.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing custom editor to BezierSpline component.
	/// Partial class providing Unity scene tools handling.
	/// </summary>
	public partial class SplineEditor : UnityEditor.Editor
	{
		/// <summary>
		/// Shows hidden Unity scene tools.
		/// </summary>
		public static void ShowTools()
		{
			if (EditorState.SavedTool == Tool.None)
			{
				EditorState.SavedTool = Tool.Move;
			}

			Tools.current = EditorState.SavedTool;
		}

		/// <summary>
		/// Hides Unity scene tools.
		/// Hidden tools are saved so they can be further restored with ShowTools method.
		/// </summary>
		public static void HideTools()
		{
			EditorState.SavedTool = Tools.current;
			Tools.current = Tool.None;
		}

		private void InitializeTools()
		{
			if (EditorState.CurrentSpline == null || EditorState.ShowTransformHandle)
			{
				ShowTools();
			}
			else
			{
				HideTools();
			}
		}

		private void ReleaseTools()
		{
			if (EditorState.CurrentSpline == null || EditorState.ShowTransformHandle)
			{
				ShowTools();
			}
			else
			{
				HideTools();
			}
		}

		private void UpdateTools()
		{
			if (EditorState.CurrentEditor == null || EditorState.CurrentSpline == null)
			{
				return;
			}

			if (EditorState.ShowTransformHandle && Tools.current == Tool.None && EditorState.SavedTool != Tool.None)
			{
				ShowTools();
			}
			else if (!EditorState.ShowTransformHandle && Tools.current != Tool.None)
			{
				HideTools();
			}
			else if (EditorState.ShowTransformHandle && Tools.current != EditorState.SavedTool)
			{
				EditorState.SavedTool = Tools.current;
			}
		}
	}
}
