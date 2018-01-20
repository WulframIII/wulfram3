//Andromeda Monitor System Editor JavaScript Version 1.3 @ Copyright
//Black Horizon Studios

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AndromedaMonitorSystem))]
public class AndromedaMonitorSystemEditor : Editor {

	AndromedaMonitorSystem t;
	
	void OnEnable (){
		t = (AndromedaMonitorSystem)target;
	}
	
	public override void OnInspectorGUI () {
		EditorGUILayout.LabelField("Andromeda Monitor System", EditorStyles.boldLabel);
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
			t.hologramSound = EditorGUILayout.ObjectField ("Hologram Sound", t.hologramSound, typeof(AudioClip), true) as AudioClip;
		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		t.hologramModel1 = EditorGUILayout.ObjectField ("Monitor Static", t.hologramModel1, typeof(GameObject), true) as GameObject;
		t.hologramModel2 = EditorGUILayout.ObjectField ("Monitor Texture", t.hologramModel2, typeof(GameObject), true) as GameObject;
		
		EditorGUILayout.Space();
		
		t.shakeIntensity = EditorGUILayout.Slider ("Shake Intensity", t.shakeIntensity, 0.0f, 1.0f);
		EditorGUILayout.Space();
		
		t.minFlicker = EditorGUILayout.Slider ("Min Static", t.minFlicker, -0.5f, 0.5f);
		t.maxFlicker = EditorGUILayout.Slider ("Max Static", t.maxFlicker, 0.0f, 2.0f);
		
		EditorGUILayout.Space();
		
		t.xSpeed = EditorGUILayout.Slider ("Static Speed X", t.xSpeed, 0.1f, 5.0f);
		t.ySpeed = EditorGUILayout.Slider ("Static Speed Y", t.ySpeed, 0.0f, 5.0f);

		EditorGUILayout.Space();
	}
	
}