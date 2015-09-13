using System;
using UnityEngine;
using UnityEditor;


public class Content {
	public readonly string contentId;

	public readonly int recordObjId;
	public RecordObject recordObj;

	public int contentData = 0;

	private Action<string> DeleteFromParent;


	/*
		Inspector for this instance.
	*/
	[CustomEditor(typeof(RecordObject))]
	public class ScriptableObjInspector : Editor {
		public override void OnInspectorGUI () {
			var recordObj = ((RecordObject)target);
			
			var recordObjId = recordObj.recordObjId;
			GUILayout.Label("recordObjId:" + recordObjId);

			var data = recordObj.data;
			GUILayout.Label("data:" + data);

			if (GUILayout.Button("update")) {
				Undo.RecordObject(recordObj, "Update");
				recordObj.data = recordObj.data + 1;
			}
		}
	}

	public Content () {
		this.contentId = Guid.NewGuid().ToString();
		
		this.recordObj = ScriptableObject.CreateInstance<RecordObject>();
		this.recordObjId = recordObj.GetInstanceID();

		recordObj.recordObjId = recordObjId;
		recordObj.data = 100;	
		Undo.RegisterCreatedObjectUndo(recordObj, "Create RecordObject:" + recordObj.GetInstanceID());
		
		ApplyRecordDataThenShouldUpdate();
	}

	public void SetActive () {
		if (recordObj == null) {
			Debug.Log("recordObj = null!");
			return;
		}
		Selection.activeObject = recordObj;
	}

	public void CountUp () {
		Undo.RecordObject(recordObj, "Update");
		recordObj.data = recordObj.data + 1;
		ApplyRecordDataThenShouldUpdate();
	}

	public bool ApplyRecordDataThenShouldUpdate () {
		if (recordObj == null) {
			var restoredRecordObj = EditorUtility.InstanceIDToObject(recordObjId) as RecordObject;

			if (restoredRecordObj != null) {
				recordObj = restoredRecordObj;
			} else {
				return false;
			}
		}

		contentData = recordObj.data;
		return true;
	}

	public bool IsValid () {
		if (recordObj == null) return false;
		return true;
	}
}