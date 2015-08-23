#UndoSample

UnityEditor undo/redo with separated classes.

##ScriptableObjectを単なるUndo/Redo保持機として扱うサンプル
ScriptableObject そのまま使ってるとUndo/Redo時のIdentifyとか設定繁栄とか融通利かなくてムカつくので、以下のように分解して保持できるようにした。

###window:
GUIの大元、全GUI要素の元締め + Undo/Redoのハンドラ保持

###content:
GUIのパーツの一つ一つのデータとビューを持てるクラス。 Undo/Redo用のScriptableObject recordObj を持つ。  
あとInspectorを持たせるとInspectorのビューもここで決着つけられて便利。

###recordObject:
ScriptableObjectの拡張。Contentと寿命を共にする。  
データの大元はContentが持っているので、Undo/Redo管理したい要素「のみ」を持たせることができる。  


##Relations
Window  
┗ Content x n  
__┗ RecordObject x n 

RecordObjectが本当にUndo/Redoの内容を小さく保持してるだけなので、Undo/Redoに巻き込みたいものを明示できて楽だ。みたいな話。


##license
MIT.