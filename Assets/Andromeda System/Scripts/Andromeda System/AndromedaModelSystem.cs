//Andromeda Model System JavaScript Version 1.3 @ Copyright
//Black Horizon Studios

using UnityEngine;
using System.Collections;

public class AndromedaModelSystem : MonoBehaviour {
	public float floatUpSpeed = 0.01f; 
	public float floatDownSpeed = 0.01f; 
	public float shakeIntensity = 0.1f;
	
	public GameObject hologramModel;
	Renderer hologramModelRenderer;
	
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
	
	public bool  useLight = false;
	public bool  useSound = false;

	AudioSource AS;
	float YShake;
	
	void  Awake (){
		AS = GetComponent<AudioSource>();
		AS.clip = hologramSound;
		AS.enabled = false;

		hologramModelRenderer = hologramModel.GetComponent<Renderer>();
	}
	
	void  Start (){
		floatup = false;
		
		if (useSound)
		{
			AS.enabled = true;
		}		
		
		if (hologramModel == null)
			Debug.LogError("You need to apply a model to the Hologram Model slot. The model must contain 1st the the Hologram Static Shader then the Hologram Solid Shader. Refer to the demo scene for an example if needed.");
		
	}
	
	void  Update (){
		transform.Rotate(Vector3.right * rotateSpeed);
		transform.Rotate(Vector3.up * rotateSpeed, Space.World);
		Quaternion Rot = transform.rotation;
		Rot.eulerAngles = new Vector3 (0,transform.rotation.eulerAngles.y,0);
		transform.rotation = Rot;
		
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
					AS.volume = flickerSpeed + 0.3f;
					
					StartCoroutine("Delay");
				}
				
				if (flickerSpeed < 0.25f)
				{
					AS.enabled = false;
				}
			}
		}

        foreach (Material m in hologramModelRenderer.materials)
        {
            m.SetFloat("_Mode", 3f);
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 1);
            m.DisableKeyword("_ALPHATEST_ON");
            m.DisableKeyword("_ALPHABLEND_ON");
            m.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = 3000;
            m.color = new Color(0, 0.8f, 0.85f, flickerSpeed * 0.25f); // TempColor;
        }

        //hologramModelRenderer.material.mainTextureOffset = new Vector2 (offsetX,offsetY); 
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