using UnityEngine;
using System.Collections.Generic;
using System;

namespace Player
{
    /// <summary>
    /// Utility class to facilitate player behaviour
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {

        public GameObject PlayerObjectPrefab;

        private static PlayerManager h_Instance;
        private List<PlayerInfo> m_PlayerInfoList;
        private List<Color> m_SlotColorList;

        // Use this for initialization
        void Start()
        {

            if (PlayerObjectPrefab == null)
                Debug.LogError("PlayerObjectPrefab has not been assignned through the unity Editor!");

            h_Instance = this;
            m_PlayerInfoList = new List<PlayerInfo>(4) { new PlayerInfo(), new PlayerInfo(), new PlayerInfo(), new PlayerInfo() };

            // set default player colors
            m_SlotColorList = new List<Color>(4)
            {
                new Color( 255f / 255f , 69f/255f , 0f ), // orange/red
                new Color( 0f / 255f , 86f/255f , 255f/255f ), // blueish
                new Color( 104f / 255f , 255f/255f , 0f/255f ), // greenish
                new Color( 255f / 255f , 248f/255f , 0f/255f ), // yellowish
            };

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Get player manager component
        /// </summary>
        /// <returns></returns>
        public static PlayerManager GetInstance()
        {
            // If h_Instance has never been called, find it, and assign it
            if (h_Instance == null)
            {
                GameObject playerManagerObject = GameObject.Find("Players");
                PlayerManager instance = playerManagerObject.GetComponent<PlayerManager>();
                h_Instance = instance;
            }
            return h_Instance;
        }


        /// <summary>
        /// Adds a Player to the game List
        /// </summary>
        /// <param name="photonPlayerID"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [PunRPC]
        public bool AddPlayerToPlayerManager(int photonPlayerID, int index)
        {
            bool succes = false;

            // find player in connected clinets list and write playerInfo to the list of playerinfo's
            PhotonPlayer[] players = PhotonNetwork.playerList;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].ID == photonPlayerID)
                {
                    PhotonPlayer photonPlayer = players[i];
                    PlayerInfo pInfo = new PlayerInfo();

                    pInfo.PhotonPlayer = photonPlayer;
                    pInfo.PlayerObject = null;
                    pInfo.SlotID = index;

                    // add playerInfo to players List
                    m_PlayerInfoList[index] = (pInfo);

                    // Assign player to Scoreboard slot
                    ScoreBoard.GetInstance().AssignPlayerToSlot(photonPlayer, index);

                    // Log message to players
                    string coloredPlayerName = ColorUtility.ColorRichtText(m_SlotColorList[index], photonPlayer.name);
                    EventLog.GetInstance().LogMessage("<b>" + coloredPlayerName + "</b> has connected!");

                    succes = true;
                }
            }


            return succes;
        }

        /// <summary>
        /// Assigns a Player object to a photonPlayer
        /// </summary>
        /// <param name="playerObject"></param>
        /// <param name="photonPlayer"></param>
        public void AssignPlayerObjectToPlayer(PlayerController playerObject, PhotonPlayer photonPlayer)
        {
            for (int i = 0; i < m_PlayerInfoList.Count; i++)
            {
                if (m_PlayerInfoList[i].PhotonPlayer == null)
                    continue;

                if (m_PlayerInfoList[i].PhotonPlayer.ID == photonPlayer.ID)
                {
                    PlayerInfo plInfo = m_PlayerInfoList[i];
                    if (plInfo.PlayerObject != null)
                        Debug.Log(PhotonNetwork.player.name + " says: I already have a player object, yet I'm assigning one. Is it the same one?");

                    plInfo.PlayerObject = playerObject;

                    m_PlayerInfoList[i] = plInfo;
                }

            }
        }

        /// <summary>
        /// Returns the PhotonPlayer assigned to a given slot. Returns null if slot is empty
        /// </summary>
        /// <param name="slotIndex"></param>
        /// <returns></returns>
        public PhotonPlayer GetPlayerInSlot(int slotIndex)
        {
            PhotonPlayer pPlayer = m_PlayerInfoList[slotIndex].PhotonPlayer;
            return pPlayer;

        }

        public int GetPlayerSlot(PhotonPlayer player)
        {
            for (int i = 0; i < m_PlayerInfoList.Count; i++)
            {
                if (m_PlayerInfoList[i].PhotonPlayer == null)
                    continue;
                if (m_PlayerInfoList[i].PhotonPlayer.ID == player.ID)
                    return m_PlayerInfoList[i].SlotID;
            }
            return -1;
        }

        /// <summary>
        /// Returns the playerobject owned by this client
        /// </summary>
        /// <returns></returns>
        public PlayerController GetLocalPlayerObject()
        {
            for (int i = 0; i < m_PlayerInfoList.Count; i++)
            {
                if (m_PlayerInfoList[i].PhotonPlayer == null)
                    continue;
                if (m_PlayerInfoList[i].PhotonPlayer.isLocal)
                    return m_PlayerInfoList[i].PlayerObject;
            }
            return null;
        }


        public PlayerController GetPlayerObjectInSlot(int slotIndex)
        {
            Debug.LogError("GetPlayerObjectInSlot: Not Implemented yet!");
            return null;
        }

        /// <summary>
        /// Returns all player objects
        /// </summary>
        /// <returns></returns>
        public List<PlayerController> GetAllPlayerObjects()
        {
            List<PlayerController> playerList = new List<PlayerController>();
            for (int i = 0; i < m_PlayerInfoList.Count; i++)
            {
                playerList.Add(m_PlayerInfoList[i].PlayerObject);

            }
            return playerList;
        }


        public PlayerController GetPlayerObject(PhotonPlayer photonPlayer)
        {
            for (int i = 0; i < m_PlayerInfoList.Count; i++)
            {
                if (m_PlayerInfoList[i].PhotonPlayer != null)
                {
                    if (m_PlayerInfoList[i].PhotonPlayer.ID == photonPlayer.ID)
                    {
                        return m_PlayerInfoList[i].PlayerObject;
                    }
                }
            }
            return null;
        }

        public Color GetPlayerSlotColor(int slotID)
        {
            return m_SlotColorList[slotID];
        }


        public int GetOpenPlayerSlot()
        {
            for (int i = 0; i < m_PlayerInfoList.Count; i++)
            {
                if (m_PlayerInfoList[i].PhotonPlayer == null)
                    return i;
            }
            return -1;
        }



        /// <summary>
        /// Clears player slot data
        /// </summary>
        /// <param name="photonPlayerID"></param>
        public void RemovePlayerFromPlayerManager(PhotonPlayer photonPlayer)
        {
            // copy of left player data
            //PlayerInfo leftPlayerInfo = new PlayerInfo();

            // remove player data from own record
            for (int i = 0; i < m_PlayerInfoList.Count; i++)
            {
                if (m_PlayerInfoList[i].PhotonPlayer == null) // player has quit == null
                {
                    Debug.LogWarning("No player found to clean up!");
                    m_PlayerInfoList[i] = new PlayerInfo(); // overwrite slot with null data
                }
                else if (m_PlayerInfoList[i].PhotonPlayer.ID == photonPlayer.ID)
                {
                    m_PlayerInfoList[i] = new PlayerInfo(); // overwrite slot with null data
                }


            }

            // Clear Player slot
            // find a player slot that is NOT 0


            //ScoreBoard.GetInstance().ClearPlayerFromSlot(leftPlayerInfo.SlotID); 
            ScoreBoard.GetInstance().RemovePlayer(photonPlayer);


            // despawn player if not despawned already

            // Write message that a player has left
            //Color slotColor = GetPlayerSlotColor(leftPlayerInfo.SlotID);

            string dcMessage = "<b>" + photonPlayer.name + "</b>" + " has disconnected.";

            EventLog.GetInstance().LogMessage(dcMessage);

            Debug.LogWarning("PlayerController Disconnect is NOT fully implemented yet.");
        }




        //------------------------
        // Player Object Interface
        //------------------------

        /// <summary>
        /// Creates new player object and fully initiates them
        /// </summary>
        public void SpawnPlayers_All()
        {
            // Loop & Spawn with default settings
            for (int i = 0; i < m_PlayerInfoList.Count; i++)
            {
                if (m_PlayerInfoList[i].PhotonPlayer != null)
                    SpawnPlayer_Single(m_PlayerInfoList[i].PhotonPlayer);
            }

        }

        /// <summary>
        /// Creates a new playerObject and fully initiates it, spawns it on an open spawn field, returns the object
        /// </summary>
        /// <param name="player"></param>
        public PlayerController SpawnPlayer_Single(PhotonPlayer player)
        {
            PlayerController newPlayerObject = null;

            // new object creation
            if (PlayerObject_Exists(player) == false)
            {
                // allow the CLIENT to create its own player
                if (player.isLocal)
                {
                    GameObject newObject = PhotonNetwork.Instantiate(PlayerObjectPrefab.name, Vector3.zero, Quaternion.identity, 0);
                    newPlayerObject = newObject.GetComponent<PlayerController>();
                    newPlayerObject.Photonplayer = player; // important!
                    newPlayerObject.transform.SetParent(this.transform);
                    AssignPlayerObjectToPlayer(newPlayerObject, player); // VERY important!
                }
                else return null; // Exit, because I don't own this player
            }
            else newPlayerObject = GetPlayerObject(player);


            // object initialisation & position
            //Position
            int playerSlotIndex = GetPlayerSlot(player);
            Vector3 spawnPosition = PlayerSpawns.GetInstance().SpawnList[playerSlotIndex].position;
            newPlayerObject.transform.position = spawnPosition;


            //DEBUG:
            newPlayerObject.GetComponentInChildren<Renderer>().material.color = m_SlotColorList[GetPlayerSlot(player)];

            return newPlayerObject;
        }

        public void AddPlayerCondition_All(Type conditionType)
        {
            // Freeze all players
            for (int i = 0; i < m_PlayerInfoList.Count; i++)
            {
                PlayerController playerComp = m_PlayerInfoList[i].PlayerObject;
                if (playerComp == null)
                    continue;
                //Debug.Log("Conditions Object: " + playerComp));      
                playerComp.Conditions.AddCondition(conditionType);


            }
        }

        public void RemovePlayerConditions_All()
        {
            // Freeze all players
            for (int i = 0; i < m_PlayerInfoList.Count; i++)
            {
                PlayerController playerComp = m_PlayerInfoList[i].PlayerObject;

                if (playerComp == null)
                    continue;

                playerComp.Conditions.RemoveAllConditions();


            }
        }
        [System.Obsolete("Moved to AddPlayerCondition_All")]
        public void SetPlayerFrozen_All(bool frozen_position, bool frozen_lookat)
        {
            // Freeze all players
            for (int i = 0; i < m_PlayerInfoList.Count; i++)
            {
                PlayerController playerComp = m_PlayerInfoList[i].PlayerObject;
                if (playerComp == null)
                    continue;

                if(frozen_position)
                    playerComp.Conditions.AddCondition(typeof(Condition_Frozen));
                else playerComp.Conditions.RemoveCondition( typeof(Condition_Frozen));


            }
        }

        public void DeSpawnPlayer(PhotonPlayer photonPlayer) { throw new System.Exception("Not Implemented Yet!"); }

        /// <summary>
        /// Checks if a player object exists for this player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerObject_Exists(PhotonPlayer player)
        {
            // find ID, then nullcheck PlayerObject
            for (int i = 0; i < m_PlayerInfoList.Count; i++)
            {
                if (m_PlayerInfoList[i].PhotonPlayer == null)
                    continue;
                if (m_PlayerInfoList[i].PhotonPlayer.ID == player.ID)
                {
                    if (m_PlayerInfoList[i].PlayerObject != null)
                        return true;
                }
            }
            return false;
        }
    }

    public struct PlayerInfo
    {
        public int SlotID;
        public PhotonPlayer PhotonPlayer;
        public PlayerController PlayerObject;
    }
}