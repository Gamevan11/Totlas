using UnityEngine;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival.Editor
{
	using UnityEditor;

	[CustomEditor(typeof(BuildingPiece))]
	[CanEditMultipleObjects]
	public class BuildingPieceEditor : Editor 
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Space();

			if(GUI.changed)
				Apply();
		}

		private void OnEnable()
		{
			Apply();
            EditorApplication.hierarchyChanged += OnHierarchyChange;
		}

		private void OnHierarchyChange()
		{
			Apply();
		}

		private void OnDestroy()
		{
            EditorApplication.hierarchyChanged -= OnHierarchyChange;
		}

		private void Apply()
		{
			var category = (serializedObject.targetObject as BuildingPiece).GetComponentInParent<BuildingCategory>();
			if(category != null)
				BuildingCategoryEditor.ApplyForCategory(category);
		}
	}
}
