using Com.Wulfram3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GridDetectionManager : MonoBehaviour {


    public Terrain loadedTerrain;

    public GridSector currentPlayerSector;

    public Text locationtext;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(PlayerMovementManager.LocalPlayerInstance == null)
        {
            return;
        }
        var worldPos = PlayerMovementManager.LocalPlayerInstance.transform.position;
        var terrainLocalPos = worldPos - loadedTerrain.transform.position;
        var normalizedPos = new Vector2(Mathf.InverseLerp(0.0f, loadedTerrain.terrainData.size.x, terrainLocalPos.x),
                                    Mathf.InverseLerp(0.0f, loadedTerrain.terrainData.size.z, terrainLocalPos.z));
        var terrainNormal = loadedTerrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);
        //Debug.Log(terrainNormal + " " + normalizedPos.x + " " + normalizedPos.y);
        this.currentPlayerSector = this.GetSector(normalizedPos);
        if(!string.IsNullOrEmpty(this.currentPlayerSector.Sector) && locationtext != null)
        {
            //Debug.Log(this.currentPlayerSector.Sector);
            if (locationtext.text != this.currentPlayerSector.Sector)
            {
                locationtext.text = this.currentPlayerSector.Sector;
            }
        } 
    }

    private GridSector GetSector(Vector2 pos)
    {
        var roundedX = Math.Round(pos.x, 2); // Horizontal
        var roundedY = Math.Round(pos.y, 2); // Vertical
        var result = new GridSector();

        if(roundedX.IsWithin(0.00, 0.17))
        {
            result.HorizontalSector = "A";
        }
        else if (roundedX.IsWithin(0.18, 0.34))
        {
            result.HorizontalSector = "B";
        }
        else if (roundedX.IsWithin(0.36, 0.51))
        {
            result.HorizontalSector = "C";
        }
        else if (roundedX.IsWithin(0.52, 0.68))
        {
            result.HorizontalSector = "D";
        }
        else if (roundedX.IsWithin(0.69, 0.85))
        {
            result.HorizontalSector = "E";
        }
        else if (roundedX.IsWithin(0.86, 1))
        {
            result.HorizontalSector = "F";
        }



        if (roundedY.IsWithin(0.00, 0.17))
        {
            result.VerticalSector = "1";
        }
        else if (roundedY.IsWithin(0.18, 0.34))
        {
            result.VerticalSector = "2";
        }
        else if (roundedY.IsWithin(0.36, 0.51))
        {
            result.VerticalSector = "3";
        }
        else if (roundedY.IsWithin(0.52, 0.68))
        {
            result.VerticalSector = "4";
        }
        else if (roundedY.IsWithin(0.69, 0.85))
        {
            result.VerticalSector = "5";
        }
        else if (roundedY.IsWithin(0.86, 1))
        {
            result.VerticalSector = "6";
        }
        
        return result;
    }


}

public static class Ext
{
    public static bool IsWithin<T>(this T value, T minimum, T maximum) where T : IComparable<T>
    {
        if (value.CompareTo(minimum) < 0)
            return false;
        if (value.CompareTo(maximum) > 0)
            return false;
        return true;
    }
}