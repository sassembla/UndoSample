using System;
using UnityEngine;
using UnityEditor;

[Serializable] public class PointObject {
	[SerializeField] private PointInspector inspectObj;

	[SerializeField] private int data = 0;

	/*
		Inspector for this instance.
	*/
	[CustomEditor(typeof(PointInspector))]
	public class ContentInspectorGUI : Editor {
		public override void OnInspectorGUI () {
			var inspectObj = ((PointInspector)target);
			GUILayout.Label("inspectObj.data:" + inspectObj.data);
		}
	}

	public void ApplyDataToInspector () {
		if (inspectObj == null) inspectObj = ScriptableObject.CreateInstance("PointInspector") as PointInspector;

		inspectObj.data = data;
	}
}