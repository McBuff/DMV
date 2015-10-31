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

    public GameObject PlayerList;
    public GameObject SpawnpointList;

    public string PlayerName = "Player Name";
    public GameState m_CurrentGamestate;
    
 

    // Use this for initialization
    void Start () {
        Debug.Log("Initting");
        PhotonNetwork.ConnectUsingSettings("0.3");
        
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
                    if (GUI.Button(new Rect(100, 250 + (95 * i), 500, 90), "Join " + roomsList[i].name))
                        PhotonNetwork.JoinRoom(roomsList[i].name);
                }
            }
        }

        if ( PhotonNetwork.player == PhotonNetwork.masterClient)
        {

            GUILayout.Box("ClientType: Server");
        }
        else GUILayout.Box("ClientType: Client");



    }
    void OnReceivedRoomListUpdate()
    {
        roomsList = PhotonNetwork.GetRoomList();
    }
    void OnJoinedRoom()
    {


        // debiug:
        // spawn death ball if 1 player enters
        if (PhotonNetwork.playerList.Length > 0 && PhotonNetwork.isMasterClient)
        {
            
            if(deathBall == null && PhotonNetwork.isMasterClient )
                deathBall = PhotonNetwork.Instantiate(deathBallPrefab.name, Vector3.up * .5f + Vector3.forward * 2f, Quaternion.identity, 0);
        }
        
        SpawnPlayer(PhotonNetwork.playerList.Length -1);
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
        newPlayer.transform.SetParent(PlayerList.transform);
        newPlayer.name = PlayerName;
        Color[] playerColors = new Color[4]{ Color.red, Color.green, Color.blue, Color.yellow };

        newPlayer.GetComponentInChildren<Renderer>().material.color = playerColors[ playerID];
    }
}
public enum GameState {
    PreGame,
    init,
    Playing,
    AfterMath
}
