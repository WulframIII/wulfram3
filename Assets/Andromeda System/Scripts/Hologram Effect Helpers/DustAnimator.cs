using UnityEngine;
using System.Collections;

public class DustAnimator : MonoBehaviour {

	public float xSpeed = 0; 
	public float ySpeed = 0; 
	Renderer ObjectRenderer;

	void Start (){
		ObjectRenderer = GetComponent<Renderer>();
	}

	void Update() { 
		var offsetX = Time.time * xSpeed; 
		var offsetY = Time.time * ySpeed; 
		ObjectRenderer.material.mainTextureOffset = new Vector2 (offsetX,offsetY); 
	} 
}
