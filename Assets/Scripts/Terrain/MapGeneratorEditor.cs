using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor{

	public override void OnInspectorGUI(){

		MapGenerator mapGen = (MapGenerator)target;

		//if any value changed (allows update automatically if calues changed in inspector)
		if (DrawDefaultInspector ()) {
			if (mapGen.autoUpdate) {
				mapGen.DrawMapInEditor ();
			}
		}

		if (GUILayout.Button ("generate Map")) {
			mapGen.DrawMapInEditor ();
		}
	}


}
