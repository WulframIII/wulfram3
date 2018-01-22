using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour {

    public GameObject loadingPanel;

    public GameObject background;
    private UniGifImage gif;


    // Use this for initialization
    void Start () {
        this.gif = background.GetComponent<UniGifImage>();
	}
	
	// Update is called once per frame
	void Update () {
		if(this.gif.nowState == UniGifImage.State.Playing)
        {
            loadingPanel.SetActive(false);
        }
        else
        {
            loadingPanel.SetActive(true);
        }
	}
}
