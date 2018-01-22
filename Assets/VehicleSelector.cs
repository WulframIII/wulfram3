using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSelector : MonoBehaviour {

    public Transform playerPrefab;
    public Transform[] nestedModels;
    public List<int> available;
    private int listStart = 0;
    private int listEnd = 1;
    public int current = 0;
    private int last = 0;
    public Camera myCamera;
    private float switchStamp;
    private int rotateSpeed = 25;

	// Use this for initialization
	void Start () {
        SwitchModel();
    }


    // Update is called once per frame
    void Update () { 
        if (current != last)
        {
            SwitchModel();
            last = current;
        }
        playerPrefab.RotateAround(playerPrefab.position, Vector3.up, rotateSpeed * Time.deltaTime);
	}

    public void SetAvailableModels(List<int> i)
    {
        available = i;
    }

    private void SwitchModel()
    {
        for (int i=0; i<available.Count; i++)
        {
            if (current == i)
            {
                nestedModels[available[i]].gameObject.SetActive(true);
            } else
            {
                nestedModels[available[i]].gameObject.SetActive(false);

            }
        }
    }

    public int SelectedIndex()
    {
        return available[current];
    }

    public void Next()
    {
        current++;
        if (current >= available.Count)
            current = 0;
    }

    public void Previous()
    {
        current--;
        if (current < 0)
            current = available.Count -1;
    }
}
