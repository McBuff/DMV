using UnityEngine;
using System.Collections;
using System;

public class NetworkManager : MonoBehaviour {

    // Private
    private const string roomName = "RoomName";
    private RoomInfo[] roomsList;
    public GameObject playerPrefab;
    public GameObject deathBallPrefab;
    private GameObject deathBall;
    
    public GameObject SpawnpointList;

    public string PlayerName = "Player Name";

    public int PlayerID;
    // Use this for initialization
    void Start () {
        Debug.Log("Initting Network manager!");
        PhotonNetwork.ConnectUsingSettings("0.4");
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
                PhotonNetwork.CreateRoom(roomName + Guid.NewGuid().ToString("N"), new RoomOptions() { maxPlayers = 4 }, null);

            // Join Room
            if (roomsList != null)
            {
                for (int i = 0; i < roomsList.Length; i++)
                {
                    //PhotonNetwork.player.name = "ClientPlayer";
                    if (GUI.Button(new Rect(100, 250 + (95 * i), 500, 90), "Join " + roomsList[i].name))
                        PhotonNetwork.JoinRoom(roomsList[i].name);
                }
            }
        }
        //if ( PhotonNetwork.player == PhotonNetwork.masterClient)
        //{
        //    GUILayout.Box("ClientType: Server");
        //}
        //else GUILayout.Box("ClientType: Client");




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
            PhotonNetwork.player.name = "ServerPlayer";
            EventLog.GetInstance().LogMessage("Created room! You are the host!");
        }
        else
        {
            
            PhotonNetwork.player.name = "ClientPlayer"; // NOTE: this is not the best place to rename the player. The Player's name should remain consistent throughout the joining of rooms
            EventLog.GetInstance().LogMessage("Joined room!");
        }

        // Go Into Spectating mode untill further orders have been received from the server
        // Set Lobby Cam as main camera
        
        //DELETE THIS LATER add SELF to scoreboard
        ScoreBoard.GetInstance().AssignPlayerToSlot(PhotonNetwork.player, 0);

        // CLIENT:
        PhotonPlayer[] playerList = PhotonNetwork.playerList;
        for (int i = 0; i < playerList.Length; ++i) {
            PhotonPlayer listplayer = playerList[i];

            if (listplayer.isLocal)
                PlayerID = i;
        }

        // spawn death ball if 1 player enters
        if (PhotonNetwork.playerList.Length > 0 && PhotonNetwork.isMasterClient)
        {
            
            /*
            if(deathBall == null && PhotonNetwork.isMasterClient )
                deathBall = PhotonNetwork.Instantiate(deathBallPrefab.name, Vector3.up * .5f, Quaternion.identity, 0);
                */
        }
        
        // spawn player at the end of list
        //SpawnPlayer(PhotonNetwork.playerList.Length -1);
    }

    /// <summary>
    /// Called on clients that are already connected
    /// </summary>
    /// <param name="player"></param>
    void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        EventLog.GetInstance().LogMessage("<b>" + player.name + "</b> entered the game.");
        PlayerManager.GetInstance().AddPlayerToPlayerManager(player.ID, 1);
        // Call all players to put players into an available player slot
        if( PhotonNetwork.isMasterClient)
        {
            // Send a full set of game info to the newly connected player
            // set Server pos 0
            PlayerManager.GetInstance().GetComponent<PhotonView>().RPC("AddPlayerToPlayerManager", player, new object[] { PhotonNetwork.player.ID, 0 });

            // set rest pos 1-2-3
            PlayerManager.GetInstance().GetComponent<PhotonView>().RPC("AddPlayerToPlayerManager", player, new object[] { player.ID, 1 });
        }
    }


    /// <summary>
    /// Called on clients when another client disconnects
    /// </summary>
    /// <param name="player"></param>
    void OnPhotonPlayerDisconnected( PhotonPlayer player)
    {
        // Notify player that player has disconnected


        // Clean up player info
        PlayerManager.GetInstance().RemovePlayerFromPlayerManager(player.ID);
        

    }

    /// <summary>
    /// Spawn player at A spawn point
    /// </summary>
    void SpawnPlayer(int playerID) {

        // Spawn player
        Vector3 playerPosition = Vector3.zero;

        // get a position from spawners
        playerPosition = SpawnpointList.transform.GetChild(playerID).position;

        GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.up * 1, Quaternion.identity, 0);
        newPlayer.transform.position = playerPosition;
        newPlayer.transform.SetParent(PlayerManager.GetInstance().transform );
        newPlayer.name = PlayerName;
        Color[] playerColors = new Color[4]{ Color.red, Color.green, Color.blue, Color.yellow };

        newPlayer.GetComponentInChildren<Renderer>().material.color = playerColors[ playerID];


        // CLIENT:
        PhotonPlayer[] playerList = PhotonNetwork.playerList;
        for (int i = 0; i < playerList.Length; ++i)
        {
            PhotonPlayer listplayer = playerList[i];

            if (listplayer.isLocal)
                PlayerID = i;
        }


        if ( playerID == 0)
        {
            Camera.main.GetComponent<Camera_Controller>().targetPlayer = newPlayer.transform;
        }
    }
}
//public enum GameState {
//    PreGame,
//    init,
//    Playing,
//    AfterMath
//}
