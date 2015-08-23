using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;

public class GUIWindow : EditorWindow {

	[MenuItem("Window/Test")]
	static void ShowEditor() {
		EditorWindow.GetWindow<GUIWindow>();
	}


	List<Content> contents;
	private bool repaint;


	private void OnEnable () {
		Debug.Log("window opened.");

		// handler for Undo/Redo
		Undo.undoRedoPerformed += () => {
			ApplyChanges();
		};

		// handler for updated by Inspector.
		Undo.willFlushUndoRecord += () => {
			ApplyChanges();
		};

		CreateContents();
	}


	/**
		apply changes from RecordObjects to Contents.
	*/
	private void ApplyChanges () {
		repaint = false;
		foreach (var content in contents) {
			var shouldUpdate = content.ApplyRecordDataThenShouldUpdate();
			if (shouldUpdate) repaint = true;
		}
	}

	private void CreateContents () {
		contents = new List<Content>();
		contents.Add(new Content());
	}

	int counter = 0;
	private void Update () {
		if (counter % 1000 == 0) {
			foreach (var content in contents) {
				Debug.Log("content recordObjId:" + content.recordObjId + " contentData:" + content.contentData + " IsValid:" + content.IsValid());
			}
		}

		counter++;
	}

	private void OnGUI () {
		if (GUILayout.Button("add new content-record pair")) {
			contents.Add(new Content());
		}

		EditorGUILayout.Space();

		foreach (var content in contents) {
			if (!content.IsValid()) continue;

			using (new EditorGUILayout.HorizontalScope()) {
				if (GUILayout.Button("show content recordObjId:" + content.recordObjId)) {
					content.SetActive();
				}

				if (GUILayout.Button("update count." + " current:" + content.contentData)) {
					content.CountUp();
				}
			}

			EditorGUILayout.Space();
		}

		if (repaint) HandleUtility.Repaint();
	}
}

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
