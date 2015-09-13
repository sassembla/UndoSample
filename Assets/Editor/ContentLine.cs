using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[Serializable] public class ContentLine {
	[SerializeField] private ContentInspector inspectObj;

	[SerializeField] private string id;
	[SerializeField] private int data = 0;
	[SerializeField] private bool deleted;

	[SerializeField] public List<PointObject> points = new List<PointObject>();

	private int unchangeData = 0;

	private Action<string> DeleteFromParent;


	/*
		Inspector for this instance.
	*/
	[CustomEditor(typeof(ContentInspector))]
	public class ContentInspectorGUI : Editor {
		public override void OnInspectorGUI () {
			var inspectObj = ((ContentInspector)target);
			GUILayout.Label("inspectObj.id:" + inspectObj.id);
			GUILayout.Label("inspectObj.data:" + inspectObj.data);
		}
	}

	public ContentLine () {
		// initialize structure. all data will set by record or user interaction.
	}

	public ContentLine (string id) {
		this.id = id;
		Debug.LogError("ロードからのidとか色々をセット、っていう感じ。id:" + id);
	}

	public void Delete () {
		deleted = true;
	}

	public bool IsDeleted () {
		return deleted;
	}

	public string GetId () {
		return id;
	}

	public void SetId (string id) {
		this.id = id;
	}

	public void SetActive () {
		Debug.LogError("activate");
		ApplyDataToInspector();

		Selection.activeObject = inspectObj;
	}

	public void CountUp () {
		data = data + 1;
		unchangeData = unchangeData + 1;

		ApplyDataToInspector();
	}

	public int GetData () {
		return data;
	}

	public int GetUnchangeData () {
		return unchangeData;
	}




	public void ApplyDataToInspector () {
		if (inspectObj == null) inspectObj = ScriptableObject.CreateInstance("ContentInspector") as ContentInspector;

		inspectObj.id = id;
		inspectObj.data = data;
		
		foreach (var point in points) point.ApplyDataToInspector();
	}

}