using Assets.Wulfram3.Scripts.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapModeManager : MonoBehaviour {

    public GameObject MasterHUD;

    public GameObject MapModeHUD;

    public GameObject SpawnModeHUD;

    public KGFMapSystem mapSystem;


    private MapType currentMapType;
    // Use this for initialization
    void Start () {
        this.ActivateMapMode(MapType.Mini);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if(this.currentMapType == MapType.Mini)
            {
                ActivateMapMode(MapType.Large);
            }
            else if (this.currentMapType == MapType.Large)
            {
                ActivateMapMode(MapType.Mini);
            }
        }
    }

    private void ActivateMapMode(MapType map)
    {
        this.currentMapType = map;
        switch (map)
        {
            case MapType.Mini:
                if(MasterHUD != null)
                {
                    MasterHUD.SetActive(true); 
                }

                if (MapModeHUD != null)
                {
                    MapModeHUD.SetActive(false);
                }

                if (SpawnModeHUD != null)
                {
                    SpawnModeHUD.SetActive(false);
                }

                mapSystem.SetFullscreen(false);
                Cursor.visible = false;
                break;
            case MapType.Large:
                if (MasterHUD != null)
                {
                    MasterHUD.SetActive(false);
                }

                if (MapModeHUD != null)
                {
                    MapModeHUD.SetActive(true);
                }

                if (SpawnModeHUD != null)
                {
                    SpawnModeHUD.SetActive(false);
                }

                mapSystem.SetFullscreen(true);
                Cursor.visible = true;
                break;
            case MapType.Spawn:
                if (MasterHUD != null)
                {
                    MasterHUD.SetActive(false);
                }

                if (MapModeHUD != null)
                {
                    MapModeHUD.SetActive(false);
                }

                if (SpawnModeHUD != null)
                {
                    SpawnModeHUD.SetActive(true);
                }
                Cursor.visible = true;
                break;
            default:
                break;
        }

        UpdateLockMode();
    }

    private void UpdateLockMode()
    {
        if (Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
