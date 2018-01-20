//Andromeda 2D Hologram System JavaScript Version 1.3 @ Copyright
//Black Horizon Studios

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Andromeda2DSystem : MonoBehaviour {
	public float floatUpSpeed = 0.01f; 
	public float floatDownSpeed = 0.01f; 
	public float shakeIntensity = 0.1f;
	
	public GameObject hologramPlane1;
	Renderer hologramPlane1Renderer;
	public GameObject hologramPlane2;
	Renderer hologramPlane2Renderer;
	
	public bool  doesRotate;
	public bool floatup;
	public float rotateSpeed = 0;
	
	private float flickerSpeed;
	public float minFlicker = 0;
	public float maxFlicker = 1;
	
	public float offsetX; 
	public float offsetY; 
	public float xSpeed = 1; 
	public float ySpeed = 1; 

	public Light hologramLight;
	public AudioClip hologramSound;
	
	public bool useLight = false;
	public bool useSound = false;

	AudioSource AS;
	float YShake;
	
	void  Awake (){
		AS = GetComponent<AudioSource>();
		AS.clip = hologramSound;
		AS.enabled = false;

		hologramPlane1Renderer = hologramPlane1.GetComponent<Renderer>();
		hologramPlane2Renderer = hologramPlane2.GetComponent<Renderer>();
	}
	
	void  Start (){
		floatup = false;  
		
		if (useSound)
		{
			AS.enabled = true;
		}
		
		if (hologramPlane1 == null)
			Debug.LogError("You need to apply a plane model to the Hologram Plane 1 slot. The model must contain the Hologram Shader. Refer to the demo scene for an example if needed.");
		
		if (hologramPlane2 == null)
			Debug.LogError("You need to apply a plane model to the Hologram Plane 2 slot. The model must contain the Hologram Shader. Refer to the demo scene for an example if needed.");
	}

	void  Update (){
		if(floatup)
			StartCoroutine("floatingup");
		else if(!floatup)
			StartCoroutine("floatingdown");
		
		flickerSpeed = Random.Range(minFlicker,maxFlicker);
		
		if (useLight)
		{
			hologramLight.intensity = flickerSpeed;
		}
		
		if (useSound)
		{
			if (AS.clip == hologramSound)
			{
				if (flickerSpeed > 0.25f)
				{
					AS.enabled = true;
					AS.volume = flickerSpeed + 0.3f;
					
					StartCoroutine("Delay");
				}
				
				if (flickerSpeed < 0.25f)
				{
					AS.enabled = false;
				}
			}
		}

		Color TempColor = hologramPlane1Renderer.material.color;
		TempColor.a = flickerSpeed;
		hologramPlane1Renderer.material.color = TempColor;
		hologramPlane2Renderer.material.color = TempColor;
	}

	IEnumerator  floatingup (){
		YShake += shakeIntensity * Time.deltaTime;
		transform.position = new Vector3 (transform.position.x, transform.position.y + YShake, transform.position.z);
		yield return new WaitForSeconds(floatUpSpeed);
		YShake = 0;
		floatup = false;
	}
	
	IEnumerator  floatingdown (){
		YShake += shakeIntensity * Time.deltaTime;
		transform.position = new Vector3 (transform.position.x, transform.position.y - YShake, transform.position.z);
		yield return new WaitForSeconds(floatDownSpeed);
		YShake = 0;
		floatup = true;
	}
	
	IEnumerator  Delay (){
		yield return new WaitForSeconds(0.05f);
		AS.enabled = false;
	}
}