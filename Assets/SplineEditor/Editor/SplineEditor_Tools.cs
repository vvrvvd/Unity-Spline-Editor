using UnityEditor;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		#region Static Fields

		private static Tool savedTool = Tool.None;

		#endregion

		#region Private Methods

		private void InitializeTools()
		{
			if (CurrentSpline == null || CurrentSpline.showTransformHandle)
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
			if(CurrentSpline==null || CurrentSpline.showTransformHandle)
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
			if (CurrentEditor == null || CurrentSpline == null)
			{
				return;
			}

			if(CurrentSpline.showTransformHandle && Tools.current == Tool.None && savedTool != Tool.None)
			{
				ShowTools();
			}
			else if (!CurrentSpline.showTransformHandle && Tools.current != Tool.None)
			{
				HideTools();
			}

		}

		#endregion

		#region Static Methods

		public static void ShowTools()
		{
			if (savedTool == Tool.None)
			{
				savedTool = Tool.Move;
			}

			Tools.current = savedTool;
		}

		public static void HideTools()
		{
			savedTool = Tools.current;
			Tools.current = Tool.None;
		}

		#endregion


	}

}
