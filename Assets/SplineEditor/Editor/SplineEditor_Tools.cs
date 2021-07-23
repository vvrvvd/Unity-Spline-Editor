using UnityEditor;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

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
			if(EditorState.CurrentSpline ==null || EditorState.ShowTransformHandle)
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

			if(EditorState.ShowTransformHandle && Tools.current == Tool.None && EditorState.savedTool != Tool.None)
			{
				ShowTools();
			}
			else if (!EditorState.ShowTransformHandle && Tools.current != Tool.None)
			{
				HideTools();
			} else if(EditorState.ShowTransformHandle && Tools.current != EditorState.savedTool)
			{
				EditorState.savedTool = Tools.current;
			}

		}

		public static void ShowTools()
		{
			if (EditorState.savedTool == Tool.None)
			{
				EditorState.savedTool = Tool.Move;
			}

			Tools.current = EditorState.savedTool;
		}

		public static void HideTools()
		{
			EditorState.savedTool = Tools.current;
			Tools.current = Tool.None;
		}

	}

}
