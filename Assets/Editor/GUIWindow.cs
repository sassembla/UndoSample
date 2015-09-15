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

	private Dictionary<int, string> idCacheDict = new Dictionary<int, string>();
	
	private void OnEnable () {
		Debug.LogError("onEnable");

		// handler for Undo/Redo
		Undo.undoRedoPerformed += () => {
			// restore content id from idCacheDict.
			if (idCacheDict.ContainsKey(this.contents.Count)) {
				this.contents[this.contents.Count - 1].SetId(idCacheDict[this.contents.Count]);
			}

			this.ApplyDataToInspector();

			Repaint();
		};

		var savedData = "raw data";// たとえばファイル、assetから保存データを抜き出す。

		this.ApplySavedData();
	}

	private void OnGUI () {

		if (GUILayout.Button("add content:" + this.contents.Count)) {
			
			var newContent = new ContentLine(Guid.NewGuid().ToString());

			Undo.RecordObject(this, "add");
			this.contents.Add(newContent);

			// store data to idCacheDict.
			idCacheDict[this.contents.Count] = newContent.GetId();
		}

		EditorGUILayout.Space();

		for (var i = 0; i < this.contents.Count; i++) {
			if (this.contents[i].IsDeleted()) continue;

			EditorGUILayout.BeginHorizontal();
			
			if (GUILayout.Button("SetActive")) this.contents[i].SetActive();

			if (GUILayout.Button("count up content[" + i + "]:" + this.contents[i].GetData() + " not effect:" + this.contents[i].GetUnchangeData() + " id:" + this.contents[i].GetId())) {
				Undo.RecordObject(this, "update content index:" + i);
				this.contents[i].CountUp();
			}

			if (GUILayout.Button("delete " + i)) {
				Undo.RecordObject(this, "delete:" + i);
				this.contents[i].Delete();
			}
			
			EditorGUILayout.EndHorizontal();
		}
	}


	[SerializeField] public List<ContentLine> contents;

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