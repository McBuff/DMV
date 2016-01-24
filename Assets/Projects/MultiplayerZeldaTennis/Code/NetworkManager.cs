using UnityEngine;
using System.Collections;
using System;
using Player;

public class NetworkManager : MonoBehaviour {

    // Private
    private const string roomName = "RoomName";
    private RoomInfo[] roomsList;
    public GameObject playerPrefab;
    public GameObject deathBallPrefab;
    private GameObject deathBall;
    
    public GameObject SpawnpointList;

    public string PlayerName = "PlayerController Name";

    public int PlayerID;
    // Use this for initialization
    void Start () {
        Debug.Log("Initting Network manager!");
        PhotonNetwork.ConnectUsingSettings("av1.0");
        PlayerID = -1;

    }
	
	// Update is called once per frame
	void Update () {


    }

    void OnGUI() {
        
        // network error messages
        if (!PhotonNetwork.connected)
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());            
        }

        else if (PhotonNetwork.room == null)
        {
            PlayerName = GUI.TextField(new Rect(100, 50, 100, 30), PlayerName, 20);
            // Create Room
            if (GUI.Button(new Rect(100, 100, 500, 100), "Start Server"))
            {
                PhotonNetwork.player.name = PlayerName;
                PhotonNetwork.CreateRoom(roomName + Guid.NewGuid().ToString("N"), new RoomOptions() { maxPlayers = 4 }, null);
            }

            // Join Room
            if (roomsList != null)
            {
                for (int i = 0; i < roomsList.Length; i++)
                {
                    //PhotonNetwork.player.name = "ClientPlayer";
                    if (GUI.Button(new Rect(100, 250 + (95 * i), 500, 90), "Join " + roomsList[i].name))
                    {
                        PhotonNetwork.player.name = "ClientPlayer " + Time.time;
                        //PhotonNetwork.player.name = PlayerName;
                        PhotonNetwork.JoinRoom(roomsList[i].name);
                    }
                }
            }
        }





    }
    void OnReceivedRoomListUpdate()
    {
        roomsList = PhotonNetwork.GetRoomList();
    }

    [PunRPC]
    void SetPlayerID(int playerID)
    {
        PlayerID = playerID;

    }


    /// <summary>
    /// Called on clients that have connected tot he game
    /// </summary>
    void OnJoinedRoom()
    {
        // fix sendrates for connecting client
        PhotonNetwork.sendRate = 25;
        PhotonNetwork.sendRateOnSerialize = 20;

        // Display a message to the player
        if (PhotonNetwork.masterClient == PhotonNetwork.player)
        {
            //PhotonNetwork.player.name = "ServerPlayer";
            PlayerManager.GetInstance().AddPlayerToPlayerManager(PhotonNetwork.player.ID, 0);
            EventLog.GetInstance().LogMessage("Created room! You are the host!");
        }
        else
        {
            
            //PhotonNetwork.player.name = "ClientPlayer"; // NOTE: this is not the best place to rename the player. The Player's name should remain consistent throughout the joining of rooms
            EventLog.GetInstance().LogMessage("Joined room!");
        }

        // Go Into Spectating mode untill further orders have been received from the server
        // Set Lobby Cam as main camera
        

        // spawn death ball if 1 player enters
        if (PhotonNetwork.playerList.Length > 0 && PhotonNetwork.isMasterClient)
        {
            
        }
        
    }

    void OnCreatedRoom()
    {
        
    }

    /// <summary>
    /// Called on clients that are already connected
    /// </summary>
    /// <param name="player"></param>
    void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        
        //PlayerManager.GetInstance().AddPlayerToPlayerManager(player.ID, 1);
        // Call all players to put players into an available player slot
        if( PhotonNetwork.isMasterClient)
        {            
            // set rest pos 1-2-3
            PhotonPlayer player_0 = PlayerManager.GetInstance().GetPlayerInSlot(0);
            PhotonPlayer player_1 = PlayerManager.GetInstance().GetPlayerInSlot(1);
            PhotonPlayer player_2 = PlayerManager.GetInstance().GetPlayerInSlot(2);
            PhotonPlayer player_3 = PlayerManager.GetInstance().GetPlayerInSlot(3);

            Debug.Log("A player has joined, current player slots are: " + player_0 + ", " + player_1 + ", " + player_2 + ", " + player_3);

            if (player_0 != null)
                PlayerManager.GetInstance().GetComponent<PhotonView>().RPC("AddPlayerToPlayerManager", player, new object[] { player_0.ID, 0 });
            if ( player_1 != null)
                PlayerManager.GetInstance().GetComponent<PhotonView>().RPC("AddPlayerToPlayerManager", player, new object[] { player_1.ID, 1 });
            if (player_2 != null)
                PlayerManager.GetInstance().GetComponent<PhotonView>().RPC("AddPlayerToPlayerManager", player, new object[] { player_2.ID, 2 });
            if (player_3 != null)
                PlayerManager.GetInstance().GetComponent<PhotonView>().RPC("AddPlayerToPlayerManager", player, new object[] { player_3.ID, 3 });


            // Put new player in an open slot , and inform ALL clients ( including the new one )
            //-------------------

            int openSlotID = PlayerManager.GetInstance().GetOpenPlayerSlot();
            if (openSlotID == -1)
                Debug.LogError("Server could not find a player slot to assign player to... what now?");

            Debug.Log("Assigning new player to slot: " + openSlotID);
            PlayerManager.GetInstance().GetComponent<PhotonView>().RPC("AddPlayerToPlayerManager", PhotonTargets.All, new object[] { player.ID, openSlotID });

        }

        //EventLog.GetInstance().LogMessage("<b>" + player.name + "</b> entered the game.");


    }


    /// <summary>
    /// Called on clients when another client disconnects
    /// </summary>
    /// <param name="player"></param>
    void OnPhotonPlayerDisconnected( PhotonPlayer player)
    {
        // Notify player that player has disconnected

        // Clean up player info
        PlayerManager.GetInstance().RemovePlayerFromPlayerManager(player);
        

    }
}
//public enum GameState {
//    PreGame,
//    init,
//    Playing,
//    AfterMath
//}
