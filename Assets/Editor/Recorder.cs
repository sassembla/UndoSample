using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;

/**
	このレイヤがUndoのすべてを保持する。
	このレイヤはパブリックなもののほうがいい気がする。
*/
public class Recorder : ScriptableObject {
	[SerializeField] public List<ContentLine> contents;

	/**
		保存されているデータの適応
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