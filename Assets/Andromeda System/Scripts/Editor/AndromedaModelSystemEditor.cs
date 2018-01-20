//Andromeda Model System Editor JavaScript Version 1.3 @ Copyright
//Black Horizon Studios
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AndromedaModelSystem))]
public class AndromedaModelSystemEditor : Editor {

	AndromedaModelSystem t;
	
	void OnEnable (){
		t = (AndromedaModelSystem)target;
	}
	
	public override void OnInspectorGUI () {
		EditorGUILayout.LabelField("Andromeda Model Hologram System", EditorStyles.boldLabel);
		EditorGUILayout.LabelField("By: Black Horizon Studios", EditorStyles.label);
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Hologram Options", EditorStyles.boldLabel);

		t.hologramModel = EditorGUILayout.ObjectField ("Hologram Model", t.hologramModel, typeof(GameObject), true) as GameObject;
		
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
		
		t.shakeIntensity = EditorGUILayout.Slider ("Shake Intensity", t.shakeIntensity, 0.0f, 1);
		EditorGUILayout.Space();
		
		t.minFlicker = EditorGUILayout.Slider ("Min Static", t.minFlicker, -0.5f, 0.5f);
		t.maxFlicker = EditorGUILayout.Slider ("Max Static", t.maxFlicker, 0.0f, 2.0f);
		
		EditorGUILayout.Space();
		
		t.xSpeed = EditorGUILayout.Slider ("Static Speed X", t.xSpeed, 0.1f, 5.0f);
		t.ySpeed = EditorGUILayout.Slider ("Static Speed Y", t.ySpeed, 0.0f, 5.0f);
		
		EditorGUILayout.Space();
		t.rotateSpeed = EditorGUILayout.Slider ("Rotate Speed", t.rotateSpeed, 0.0f, 1.0f);
		
		EditorGUILayout.Space();
		
		
		GUILayout.BeginHorizontal();
		GUILayout.EndHorizontal();
	}
	
}