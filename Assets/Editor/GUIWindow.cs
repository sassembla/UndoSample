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

	[SerializeField] public List<ContentLine> contents;

	private void OnEnable () {
		Debug.Log("onEnable");

		// handler for Undo/Redo
		Undo.undoRedoPerformed += () => {
			this.ApplyDataToInspector();

			Repaint();
		};

		var savedData = "raw data";// たとえばファイル、assetから保存データを抜き出す。

		this.ApplySavedData();
	}

	private void OnGUI () {

		if (GUILayout.Button("add content:" + contents.Count)) {
			
			var newContentId = Guid.NewGuid().ToString();
			var newContent = new ContentLine(newContentId);

			Undo.RecordObject(this, "Add Content id:" + newContentId);
			contents.Add(newContent);
		}

		EditorGUILayout.Space();

		for (var i = 0; i < contents.Count; i++) {
			/*
				論理削除されてる場合は無視
			*/
			if (contents[i].IsDeleted()) continue;

			var contentId = contents[i].GetId();

			EditorGUILayout.BeginHorizontal();
			
			if (GUILayout.Button("SetActive")) {
				contents[i].SetActive();
			}

			if (GUILayout.Button("count up content[" + i + "]:" + contents[i].GetData() + " not effect:" + contents[i].GetUnchangeData() + " id:" + contentId)) {
				Undo.RecordObject(this, "Update Content count, id:" + contentId);
				contents[i].CountUp();
			}

			if (GUILayout.Button("delete " + i)) {
				Undo.RecordObject(this, "Delete Content id:" + contentId);
				contents[i].Delete();
			}
			
			EditorGUILayout.EndHorizontal();
		}
	}


	/**
		保存されているデータをcontentsに適応
		ここでは初期化だけを行っている。
	*/
	public void ApplySavedData () {
		contents = new List<ContentLine>();
		var newContent = new ContentLine(Guid.NewGuid().ToString());
		contents.Add(newContent);
	}

	/**
		Undo,Redoを元に、各オブジェクトのInspectorの情報を更新する
	*/
	public void ApplyDataToInspector () {
		foreach (var content in contents) content.ApplyDataToInspector();
	}
}