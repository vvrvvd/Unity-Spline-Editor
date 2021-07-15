using UnityEditor;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		private void InitializeTools()
		{
			if (editorState.CurrentSpline == null || editorState.ShowTransformHandle)
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
			if(editorState.CurrentSpline ==null || editorState.ShowTransformHandle)
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
			if (editorState.CurrentEditor == null || editorState.CurrentSpline == null)
			{
				return;
			}

			if(editorState.ShowTransformHandle && Tools.current == Tool.None && editorState.savedTool != Tool.None)
			{
				ShowTools();
			}
			else if (!editorState.ShowTransformHandle && Tools.current != Tool.None)
			{
				HideTools();
			} else if(editorState.ShowTransformHandle && Tools.current != editorState.savedTool)
			{
				editorState.savedTool = Tools.current;
			}

		}

		public static void ShowTools()
		{
			if (editorState.savedTool == Tool.None)
			{
				editorState.savedTool = Tool.Move;
			}

			Tools.current = editorState.savedTool;
		}

		public static void HideTools()
		{
			editorState.savedTool = Tools.current;
			Tools.current = Tool.None;
		}

	}

}
