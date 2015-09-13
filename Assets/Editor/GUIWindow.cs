using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;

public class GUIWindow : EditorWindow {
	[MenuItem("Window/UndoTest")]
	static void ShowEditor() {
		EditorWindow.GetWindow<GUIWindow>();
	}

	private Recorder record;
	private int recordId;

	private Dictionary<int, string> idCacheDict = new Dictionary<int, string>();
	
	private void OnEnable () {
		Debug.LogError("onEnable");

		// handler for Undo/Redo
		Undo.undoRedoPerformed += () => {
			// restore content id from idCacheDict.
			if (idCacheDict.ContainsKey(record.contents.Count)) {
				record.contents[record.contents.Count - 1].SetId(idCacheDict[record.contents.Count]);
			}

			record.ApplyDataToInspector();

			Repaint();
		};

		var savedData = "raw data";// 抽出した結果を保存しておく。それを引き出す。

		RestoreRecord();
	}

	private void OnGUI () {
		if (record == null) {
			Debug.LogError("regenerate.");
			var candidate = EditorUtility.InstanceIDToObject(recordId) as Recorder;

			if (candidate == null) RestoreRecord();
			else record = candidate;
		}


		if (GUILayout.Button("add content:" + record.contents.Count)) {
			
			var newContent = new ContentLine(Guid.NewGuid().ToString());

			Undo.RecordObject(record, "add");
			record.contents.Add(newContent);

			// store data to idCacheDict.
			idCacheDict[record.contents.Count] = newContent.GetId();
		}

		EditorGUILayout.Space();

		for (var i = 0; i < record.contents.Count; i++) {
			if (record.contents[i].IsDeleted()) continue;

			EditorGUILayout.BeginHorizontal();
			
			if (GUILayout.Button("SetActive")) record.contents[i].SetActive();

			if (GUILayout.Button("count up content[" + i + "]:" + record.contents[i].GetData() + " not effect:" + record.contents[i].GetUnchangeData() + " id:" + record.contents[i].GetId())) {
				Undo.RecordObject(record, "update content index:" + i);
				record.contents[i].CountUp();
			}

			if (GUILayout.Button("delete " + i)) {
				Undo.RecordObject(record, "delete:" + i);
				record.contents[i].Delete();
			}
			
			EditorGUILayout.EndHorizontal();
		}
	}

	private void RestoreRecord () {
		if (record == null) {
			record = ScriptableObject.CreateInstance("Recorder") as Recorder;
			recordId = record.GetInstanceID();
		}

		/*
			保存されているデータを読み出して使用する。
		*/
		record.ApplySavedData();
	}
}