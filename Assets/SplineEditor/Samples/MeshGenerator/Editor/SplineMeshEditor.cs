using System;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	[CustomEditor(typeof(SplineMesh))]
	public class SplineMeshEditor : UnityEditor.Editor
	{

		private SplineMesh splineMesh;

		private Vector3[] segmentPoints;

		private void OnEnable()
		{
			splineMesh = target as SplineMesh;
			segmentPoints = new Vector3[splineMesh.segmentsCount+1];
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Generate Mesh"))
			{
				splineMesh.ConstructMesh();
			}
		}

		//private void OnSceneGUI()
		//{
		//	if(splineMesh.BezierSpline==null)
		//	{
		//		return;
		//	}

		//	if((splineMesh.BezierSpline.IsLoop && segmentPoints.Length != splineMesh.segmentsCount) || (!splineMesh.BezierSpline.IsLoop && segmentPoints.Length != splineMesh.segmentsCount+1))
		//	{
		//		var newArraySize = splineMesh.BezierSpline.IsLoop ? splineMesh.segmentsCount : splineMesh.segmentsCount + 1;
		//		Array.Resize(ref segmentPoints, newArraySize);
		//	}

		//	splineMesh.BezierSpline.GetEvenlySpacedPointsNonAlloc(splineMesh.segmentsCount, segmentPoints, splineMesh.precision);

		//	Handles.color = Color.blue;
		//	for(var i=0; i<segmentPoints.Length; i++)
		//	{
		//		var point = segmentPoints[i];
		//		var size = HandleUtility.GetHandleSize(point);
		//		Handles.Button(point, Quaternion.identity, size*0.1f, size*0.1f, Handles.DotHandleCap);
		//	}
		//}
	}

}
