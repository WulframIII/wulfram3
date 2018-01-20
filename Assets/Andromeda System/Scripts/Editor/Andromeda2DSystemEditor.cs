//Andromeda Model System JavaScript Version 1.3 @ Copyright
//Black Horizon Studios

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Andromeda2DSystem))]
public class Andromeda2DSystemEditorC : Editor {

	Andromeda2DSystem t;

	void OnEnable (){
		t = (Andromeda2DSystem)target;
	}

	public override void OnInspectorGUI () {
		EditorGUILayout.LabelField("Andromeda 2D Hologram System", EditorStyles.boldLabel);
		EditorGUILayout.LabelField("By: Black Horizon Studios", EditorStyles.label);
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Hologram Options", EditorStyles.boldLabel);
		
		EditorGUILayout.Space();
		
		t.useLight = EditorGUILayout.Toggle ("Use Light?",t.useLight);
		
		EditorGUILayout.Space();
		
		if (t.useLight)
		{
			EditorGUILayout.HelpBox("While 'Use Light' is checked Andromeda will flicker a light based off the filcker speed.", MessageType.Info, true);
			t.hologramLight = EditorGUILayout.ObjectField ("Hologram Light", t.hologramLight, typeof(Light), true) as Light;
		}
		
		EditorGUILayout.Space();
		
		t.useSound = EditorGUILayout.Toggle ("Use Sound?",t.useSound);
		
		EditorGUILayout.Space();
		
		if (t.useSound)
		{
			EditorGUILayout.HelpBox("While 'Use Sound' is checked Andromeda will play a flickering sound based off the flicker speed.", MessageType.Info, true);
			t.hologramSound = EditorGUILayout.ObjectField ("Hologram Sound", t.hologramSound, typeof(AudioClip), true) as AudioClip;
		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		t.hologramPlane1 = EditorGUILayout.ObjectField ("Hologram Plane 1", t.hologramPlane1, typeof(GameObject), true) as GameObject;
		t.hologramPlane2 = EditorGUILayout.ObjectField ("Hologram Plane 2", t.hologramPlane2, typeof(GameObject), true) as GameObject;
		
		EditorGUILayout.Space();
		
		t.shakeIntensity = EditorGUILayout.Slider ("Shake Intensity", t.shakeIntensity, 0.0f, 1f);
		EditorGUILayout.Space();
		
		t.minFlicker = EditorGUILayout.Slider ("Min Flicker", t.minFlicker, -0.5f, 0.5f);
		t.maxFlicker = EditorGUILayout.Slider ("Max Flicker", t.maxFlicker, 0.0f, 2.0f);
		
		EditorGUILayout.Space();

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		
		GUILayout.BeginHorizontal();
		
		
		GUILayout.EndHorizontal();
	}
}
