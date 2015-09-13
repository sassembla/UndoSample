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