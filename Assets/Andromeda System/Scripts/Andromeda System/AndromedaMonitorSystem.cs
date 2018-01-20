//Andromeda Monitor System JavaScript Version 1.3 @ Copyright
//Black Horizon Studios

using UnityEngine;
using System.Collections;

public class AndromedaMonitorSystem : MonoBehaviour {
	public float floatUpSpeed = 0.01f;
	public float floatDownSpeed = 0.01f;
	public float shakeIntensity = 0.1f;
	
	public GameObject hologramModel1;
	public Renderer hologramModel1Renderer;
	public GameObject hologramModel2;
	public Renderer hologramModel2Renderer;
	
	public bool  doesRotate;
	public float rotateSpeed = 0;
	private bool floatup;
	
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

		hologramModel1Renderer = hologramModel1.GetComponent<Renderer>();
		hologramModel2Renderer = hologramModel2.GetComponent<Renderer>();
	}
	
	void  Start (){
		floatup = false;  
		
		if (useSound)
		{
			AS.enabled = true;
		}
		
		if (hologramModel1 == null)
			Debug.LogError("You need to apply a plane model to the Monitor Static slot. The model must contain the Hologram Static Shader. Refer to the demo scene for an example if needed.");
		
		if (hologramModel2 == null)
			Debug.LogError("You need to apply a plane model to the Monitor Texture slot. The model must contain the Hologram Shader. Refer to the demo scene for an example if needed.");
	}
	void  Update (){
		
		offsetX = Time.time * xSpeed; 
		offsetY = Time.time * ySpeed; 
		
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
					AS.volume = flickerSpeed;
					
					StartCoroutine("Delay");
				}
				
				if (flickerSpeed < 0.25f)
				{
					AS.enabled = false;
				}
			}
		}

		Color TempColor = hologramModel1Renderer.material.color;
		TempColor.a = flickerSpeed;
		hologramModel1Renderer.material.color = TempColor;
		hologramModel2Renderer.material.color = TempColor;
			
		hologramModel1Renderer.material.mainTextureOffset = new Vector2 (offsetX,offsetY); 
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
