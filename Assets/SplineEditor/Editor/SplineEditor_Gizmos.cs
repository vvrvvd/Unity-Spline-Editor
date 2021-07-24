// <copyright file="SplineEditor_Gizmos.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing custom editor to BezierSpline component.
	/// Partial class providing custom gizmos draws on scene view.
	/// </summary>
	public partial class SplineEditor : UnityEditor.Editor
	{
		[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
		private static void RenderCustomGizmo(BezierSpline curve, GizmoType gizmoType)
		{
			if (!EditorState.AlwaysDrawSplineOnScene || !EditorState.DrawSpline || EditorState.CurrentSpline == curve)
			{
				return;
			}

			DrawSpline(curve);
		}
	}
}
