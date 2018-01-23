using Assets.Wulfram3.Scripts.InternalApis;
using Assets.Wulfram3.Scripts.InternalApis.Classes;
using Assets.Wulfram3.Scripts.InternalApis.Interfaces;
using Assets.Wulfram3.Scripts.Units;
using System.Collections.Generic;


using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.Wulfram3
{
    public class GameManager : Photon.PunBehaviour
    {
        public GameObject hullBar;

        public GameObject fuelBar;

        public Material redcolor;
        public Material bluecolor;
        public Material graycolor;

        public GameObject playerInfoPanelPrefab;
        public Transform[] spawnPointsBlue;
        public Transform[] spawnPointsRed;

        [HideInInspector]
        public Camera normalCamera;

        private TargetInfoController targetChangeListener;

        private bool isFirstSpawn = true;

        public VehicleSelector unitSelector;

        #region Photon Messages




        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                //LoadArena();
            }
        }


        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                //LoadArena();
            }
        }


        #endregion


        #region Public Methods


        public void LeaveRoom()
        {
            var userControler = DepenencyInjector.Resolve<IUserController>();
            var discordApi = DepenencyInjector.Resolve<IDiscordApi>();
            PhotonNetwork.LeaveRoom();
            StartCoroutine(discordApi.PlayerLeft(PhotonNetwork.playerName));
        }

        public void Start()
        {
            Debug.Log("GameManager.cs Start() " + Application.loadedLevelName);

            if (PlayerMovementManager.LocalPlayerInstance == null) {
                Debug.Log("We are Instantiating LocalPlayer from " + Application.loadedLevelName);

                GameObject g = Instantiate(Resources.Load("VehicleSelector"), new Vector3(-500, -500, -500), Quaternion.identity, transform) as GameObject;
                unitSelector = g.GetComponent<VehicleSelector>();
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate


                //team start
                PunTeams.UpdateTeamsNow();

                int redPlayers = PunTeams.PlayersPerTeam[PunTeams.Team.Red].Count;
                int bluePlayers = PunTeams.PlayersPerTeam[PunTeams.Team.Blue].Count;

                Debug.Log("Number of Red players: " + redPlayers);
                Debug.Log("Number of Blue players: " + bluePlayers);
                GameObject player;
                /* TODO: 
                 * Sort Respawn Code
                 * Detect player tags and modify starting prefab choices 
                 
                 */

                object[] o = new object[2];
                o[0] = UnitType.Tank;
                List<int> availableUnits = new List<int>();
                if (bluePlayers > redPlayers) {
                    o[1] = PunTeams.Team.Red;
                    availableUnits.Add(2);
                    availableUnits.Add(3);
                }
                else {
                    o[1] = PunTeams.Team.Blue;
                    availableUnits.Add(0);
                    availableUnits.Add(1);
                }
                if (PhotonNetwork.player.name.Contains("[DEV]"))
                {
                    availableUnits.Add(4);
                }


                Debug.Log("Assigned to " + o[1] + " team. Awaiting first spawn.");
                unitSelector.SetAvailableModels(availableUnits);
                player = PhotonNetwork.Instantiate("Unit_Prefabs/Player/Player", new Vector3(0, -100, 0), Quaternion.identity, 0, o);
                PhotonNetwork.player.SetTeam((PunTeams.Team)o[1]);
            }
            else {
                Debug.Log("Ignoring scene load for " + Application.loadedLevelName);
            }
            //PlayerSpawnManager.status = SpawnStatus.IsAlive;
            GetComponent<PlayerSpawnManager>().StartSpawn();
            GetComponent<MapModeManager>().ActivateMapMode(MapType.Spawn);
            //normalCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();          
        }

        public void NextPreview()
        {
            unitSelector.Next();
        }

        public void PreviousPreview()
        {
            unitSelector.Previous();
        }

        private void Update()
        {
           /*
            if(isFirstSpawn)
            {
                PlayerMovementManager player = PlayerMovementManager.LocalPlayerInstance.GetComponent<PlayerMovementManager>();
                if (player != null)
                {
                    //player.PrepareForRespawn();
                    //this.Respawn(null);
                    isFirstSpawn = false;
                }
            } */
        }

        [PunRPC]
        public void SpawnPulseShell(object[] args)
        {
            if (PhotonNetwork.isMasterClient)
            {
                object[] instanceData = new object[2];
                instanceData[0] = (PunTeams.Team) args[2];
                instanceData[1] = UnitType.Tank;
                PhotonNetwork.InstantiateSceneObject("Unit_Prefabs/Weapons/PulseShell", (Vector3) args[0], (Quaternion) args[1], 0, instanceData);
            }
        }

        public void SpawnFlakShell(Vector3 pos, Quaternion rotation, PunTeams.Team team, float fuse)
        {
            if (PhotonNetwork.isMasterClient)
            {
                object[] instanceData = new object[3];
                instanceData[0] = team;
                instanceData[1] = UnitType.FlakTurret;
                instanceData[2] = fuse;
                GameObject shell = PhotonNetwork.InstantiateSceneObject("Unit_Prefabs/Weapons/PulseShell", pos, rotation, 0, instanceData);
            }
        }

        public void SpawnExplosion(Vector3 pos)
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.InstantiateSceneObject("Effects/Explosion_01", pos, Quaternion.identity, 0, null);
            }
        }

        //laser stuff autocannon
        /*public void DrawLine(Vector3 startPos, Vector3 endPos)
		{ 
			if (PhotonNetwork.isMasterClient) {
				//PhotonNetwork.Instantiate(lineRender, startPos, Quaternion.identity, 0);

			}

		}*/


        public void UnitsHealthUpdated(HitPointsManager hitpointsManager)
        {
            if (hitpointsManager.tag.Equals("Player") && hitpointsManager.photonView.isMine)
            {
                SetHullBar((float)hitpointsManager.health / (float)hitpointsManager.maxHealth);
            }
            if (PhotonNetwork.isMasterClient && hitpointsManager.health <= 0 && !hitpointsManager.tag.Equals("Player"))
            {
                PhotonNetwork.Destroy(hitpointsManager.gameObject);
                SpawnExplosion(hitpointsManager.transform.position);
            }
        }

        public void SetHullBar(float level)
        {
            hullBar.GetComponent<LevelController>().SetLevel(level);
        }

        public void FuelLevelUpdated(FuelManager fuelManager) {
            SetFuelBar((float) fuelManager.fuel / (float) fuelManager.maxFuel);
        }

        public void SetFuelBar(float level) {
            fuelBar.GetComponent<LevelController>().SetLevel(level);
        }

        public void SetCurrentTarget(GameObject go)
        {
            if (targetChangeListener != null)
            {
                targetChangeListener.TargetChanged(go);
            }
        }

        public void AddTargetChangeListener(TargetInfoController tic)
        {
            targetChangeListener = tic;
        }

        public void DestroyNow(GameObject go)
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.Destroy(go);
                SpawnExplosion(go.transform.position);
            }
        }

        public string GetColoredPlayerName(string name, bool isMaster, bool colorFull = false, PunTeams.Team team = PunTeams.Team.none)
        {
            string masterClient = "";
            string modTag = "";
            string username = "";
            string teamColor = "";
            switch (team)
            {
                case PunTeams.Team.none:
                    teamColor = this.graycolor.color.ToHex();
                    break;
                case PunTeams.Team.Red:
                    teamColor = this.redcolor.color.ToHex();
                    break;
                case PunTeams.Team.Blue:
                    teamColor = this.bluecolor.color.ToHex();
                    break;
                default:
                    teamColor = this.graycolor.color.ToHex();
                    break;
            }


            if (isMaster)
            {
                masterClient = "<color=magenta>*</color>";
            }

            if (name.Contains("[MOD]"))
            {
                var names = name.Split(' ');

                modTag = "<color=yellow>" + names[0] + "</color>"; 
                if (colorFull == true)
                {
                    username = modTag + " " + "<color=" + teamColor + ">" + names[1] + "</color>";
                }
                else
                {
                    username = modTag + " " + "<color=white>" + names[1] + "</color>";
                }
            }
            else if (name.Contains("[DEV]"))
            {
                var names = name.Split(' ');

                modTag = "<color=#e0950b>" + names[0] + "</color>";
                if (colorFull == true)
                {
                    username = modTag + " " + "<color=" + teamColor + ">" + names[1] + "</color>";
                }
                else
                {
                    username = modTag + " " + "<color=white>" + names[1] + "</color>";
                }
            }
            else
            {
                if (colorFull == true)
                {
                    username = "<color=" + teamColor + ">" + name + "</color>";
                }
                else
                {
                    username = "<color=white>" + name + "</color>";
                }
            }

            return masterClient + username;
        }

        /*
         * 
         * MOVED: To Unit.cs
         * 
         * 
        public string PunTeamToTeamString(PunTeams.Team t)
        {
            if (t == PunTeams.Team.blue)
            {
                return "Blue";
            } else if (t == PunTeams.Team.red)
            {
                return "Red";
            } else
            {
                return "Grey";
            }
        }

        public string UnitTypeToPrefabString(UnitType u, PunTeams.Team t)
        {
            string tf = PunTeamToTeamString(t);
            string s = "Unit_Prefabs/" + tf + "/" + tf + "_";
            switch(u)
            {
                case UnitType.Cargo: s += "Cargo";  break;
                case UnitType.Darklight: s += "Darklight";  break;
                case UnitType.FlakTurret: s += "FlakTurret";  break;
                case UnitType.GunTurret: s += "GunTurret";  break;
                case UnitType.MissleLauncher: s += "Launcher"; break;
                case UnitType.PowerCell: s += "Powercell";  break;
                case UnitType.RepairPad: s += "RepairPad";  break;
                case UnitType.Skypump: s += "Skypump";  break;
                case UnitType.Tank: s += "Tank"; break;
                case UnitType.Scout: s += "Scout"; break;
                case UnitType.Uplink: s += "Uplink"; break;
                default:
                    Debug.Log("UnitTypeToPrefabString(" + u.ToString() + ", " + t.ToString() + ") ERROR: Unknown UnitType. Defaulting to cargobox!");
                    s += "Cargo";
                    break;
            }
            return s;
        }
        */

        public void Respawn(PlayerMovementManager player)
        {
            if(PlayerSpawnManager.status == SpawnStatus.IsAlive)
            {
                
                GetComponent<PlayerSpawnManager>().StartSpawn();
                GetComponent<MapModeManager>().ActivateMapMode(MapType.Spawn);
            }
        }

        /*
        [PunRPC]
        public void RequestPickUpCargo(CargoManager cargoManager) {
            if (PhotonNetwork.isMasterClient) {
                if (cargoManager.pickedUpCargo != "") {
                    return;
                }
                Cargo cargo = FindCargoInRange(cargoManager.transform.position, 5f);
                if (cargo != null) {
                    cargoManager.photonView.RPC("SetPickedUpCargo", PhotonTargets.All, cargo.content);
                    if (cargo.GetComponentInParent<PlayerMovementManager>() != null) {
                    }
                    PhotonNetwork.Destroy(cargo.gameObject);
                }
            }
        }

        private Cargo FindCargoInRange(Vector3 position, float scanRadius) {
            Transform closestTarget = null;
            float minDistance = scanRadius + 10f;
            var cols = Physics.OverlapSphere(position, scanRadius);
            var rigidbodies = new List<Rigidbody>();
            foreach (var col in cols) {
                if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody) && col.attachedRigidbody.GetComponentInParent<Cargo>() != null) {
                    rigidbodies.Add(col.attachedRigidbody);
                }
            }

            foreach (Rigidbody rb in rigidbodies) {
                Transform target = rb.transform;

                float distance = Vector3.Distance(position, target.transform.position);
                if (distance < minDistance) {
                    minDistance = distance;
                    closestTarget = target;
                }
            }

            if (closestTarget == null) {
                return null;
            }
            return closestTarget.GetComponentInParent<Cargo>();
        }

        
        public void PickUpCargo(CargoManager cargoManager) {
            photonView.RPC("RequestPickUpCargo", PhotonTargets.MasterClient, cargoManager);
        }*/

        [PunRPC]
        public void RequestDropCargo(int senderID) {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonView pv = PhotonView.Find(senderID);
                if (pv == null)
                {
                    Debug.Log("GameManager.cs --- RPC --- RequestDropCargo() Failed to find Photon View with ID: " + senderID);
                    return;
                }
                CargoManager cargoManager = pv.transform.GetComponent<CargoManager>();
                if (cargoManager != null && cargoManager.hasCargo)
                {
                    Unit u = cargoManager.transform.GetComponent<Unit>();
                    if (u != null)
                    {
                        object[] o = new object[2];
                        o[0] = cargoManager.cargoType;
                        o[1] = cargoManager.cargoTeam;
                        PhotonNetwork.InstantiateSceneObject(Unit.GetPrefabName(UnitType.Cargo, u.unitTeam), cargoManager.dropPosition.position, cargoManager.dropPosition.rotation, 0, o);
                        pv.RPC("DroppedCargo", PhotonTargets.All, null);
                    }
                }
            }
        }

        [PunRPC]
        public void RequestDeployCargo(object[] args)
        {
            if (PhotonNetwork.isMasterClient)
            {
                Vector3 desiredPosition = (Vector3)args[0];
                Quaternion desiredRotation = (Quaternion) args[1];
                UnitType cargoType = (UnitType)args[2];
                PunTeams.Team cargoTeam = (PunTeams.Team) args[3];
                object[] o = new object[2];
                o[0] = cargoType;
                o[1] = cargoTeam;
                PhotonNetwork.InstantiateSceneObject(Unit.GetPrefabName(cargoType, cargoTeam), desiredPosition, desiredRotation, 0, o);
                PhotonView.Find((int) args[4]).RPC("DeployedCargo", PhotonTargets.All, null);

            }

        }

        #endregion

        #region Private Methods

        public override void OnJoinedRoom()
        {


            Debug.Log("Sent Post!' ");
            // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.automaticallySyncScene to sync our instance scene.
            if (PhotonNetwork.room.PlayerCount == 1)
            {
                Debug.Log("We load the 'Playground' ");

                // #Critical
                // Load the Room Level. 
                PhotonNetwork.LoadLevel("Playground");

            }
        }

        void LoadArena()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }
            Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
            PhotonNetwork.LoadLevel("Playground");
        }


        #endregion


    }
}