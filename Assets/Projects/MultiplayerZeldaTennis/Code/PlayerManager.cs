using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Utility class to facilitate player behaviour
/// </summary>
public class PlayerManager : MonoBehaviour {

    private static PlayerManager h_Instance;

    private List<PlayerInfo> m_PlayerInfoList;

    private List<Color> m_SlotColorList;

	// Use this for initialization
	void Start () {

        m_PlayerInfoList = new List<PlayerInfo>(4);
        h_Instance = this;

        m_SlotColorList = new List<Color>();


        m_SlotColorList.Add(Color.red);
        m_SlotColorList.Add(Color.green);
        m_SlotColorList.Add(Color.blue);
        m_SlotColorList.Add(Color.yellow);
    }
	
	// Update is called once per frame
	void Update () {
	
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

    public PhotonPlayer GetPlayerInSlot( int slotIndex) { return null; }
    public Player GetPlayerObjectInSlot(int slotIndex) { return null; }
    public Color GetPlayerSlotColor(int slotID)
    {
        return m_SlotColorList[slotID];
    }

    
    public int GetOpenPlayerSlot()
    {
        for (int i = 0; i < m_PlayerInfoList.Count; i++)
        {
            if (m_PlayerInfoList[i].PhotonPlayer == null)
                return m_PlayerInfoList[i].SlotID;
        }
        return -1;
    }
    // returns succes/failure .. logs failure
    [PunRPC]
    public bool AddPlayerToPlayerManager(int photonPlayerID, int index) {
        bool succes = false;

        // find player in connected clinets list and write playerInfo to the list of playerinfo's
        PhotonPlayer[] players = PhotonNetwork.playerList;
        for (int i = 0; i < players.Length; i++)
        {
            if( players[i].ID == photonPlayerID)
            {
                PhotonPlayer photonPlayer = players[i];

                PlayerInfo pInfo = new PlayerInfo();

                pInfo.PhotonPlayer = photonPlayer;
                pInfo.PlayerObject = null;
                pInfo.SlotID = index;

                // add playerInfo to players List
                m_PlayerInfoList.Add(pInfo);

                // Assign player to Scoreboard slot
                ScoreBoard.GetInstance().AssignPlayerToSlot(photonPlayer, index);

                succes = true;
            }
        }


        return succes;
    }

    /// <summary>
    /// Clears player slot data
    /// </summary>
    /// <param name="photonPlayerID"></param>
    public void RemovePlayerFromPlayerManager( int photonPlayerID)
    {
        // copy of left player data
        PlayerInfo leftPlayerInfo = new PlayerInfo();

        // remove player data from own record
        for (int i = 0; i < m_PlayerInfoList.Count; i++)
        {

            if (m_PlayerInfoList[i].PhotonPlayer.ID == photonPlayerID)
            {
                leftPlayerInfo = m_PlayerInfoList[i];                
                m_PlayerInfoList[i] = new PlayerInfo(); // overwrite slot with null data
            }

        }

        // Clear Player slot
        ScoreBoard.GetInstance().ClearPlayerFromSlot(leftPlayerInfo.SlotID);

        // Write message that a player has left
        Color slotColor = GetPlayerSlotColor(leftPlayerInfo.SlotID);
        string dcMessage = "<b>" + ColorUtility.ColorToRichTextTag(slotColor) + leftPlayerInfo.PhotonPlayer.name + "</color>" + "</b> has disconnected." ;
        EventLog.GetInstance().LogMessage(dcMessage);

        Debug.LogWarning("Player Disconnect is NOT fully implemented yet.");
    }

    public struct PlayerInfo
    {
        public int SlotID;
        public PhotonPlayer PhotonPlayer;
        public Player PlayerObject;
    }
}
