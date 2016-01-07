using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBoard : MonoBehaviour {

    public RectTransform PlayerListRect;
    private static ScoreBoard h_Instance;

    private bool m_IsVisible;

    private Vector3 ScoreboardScreenPos;
	// Use this for initialization
	void Start () {
        h_Instance = this;
        ScoreboardScreenPos = this.transform.position;
        m_IsVisible = false;
    }
	
	// Update is called once per frame
	void Update () {
        
        // I've tried disabling the scoreboard, but this causes glitchy behaviour when assigning players to a slot
        // Therefore, I just offset the scoreboard Out of the screen

        Vector3 Offset = new Vector3(10000, 10000,0);

        // TAB 
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleVisibility();

        if (Input.GetKeyUp(KeyCode.Tab))
            ToggleVisibility();

        if (m_IsVisible)            
            Offset = Vector3.zero;

        transform.position = ScoreboardScreenPos + Offset;

    }

    static public ScoreBoard GetInstance()
    {
        if( h_Instance == null)
        {
            GameObject scoreBoardObject = GameObject.Find("Scoreboard");
            if (scoreBoardObject == null)
                Debug.LogError("No Scoreboard object was found! Make sure to add one in the UI Canvas!");
            else
            {
                ScoreBoard scoreboardComp = scoreBoardObject.GetComponent<ScoreBoard>();
                if (scoreboardComp == null)
                    Debug.LogError("No Scoreboard Componenet was found on an object with name Scoreboard. Make sure only 1 object is called ScoreBoard");
                else h_Instance = scoreboardComp;
            }

        }

        return h_Instance;

    }

    public void AssignPlayerToSlot(PhotonPlayer player,  int slotIndex) {

        
        // iterate over all scoreboard slots and assign player to correct slot
        ScoreBoard_Player[] scoreboardSlots = GetComponentsInChildren<ScoreBoard_Player>();
        for (int i = 0; i < scoreboardSlots.Length; i++)
        {
            if(scoreboardSlots[i].SlotIndex == slotIndex)
            {
                scoreboardSlots[i].AssignedPlayer = player;
            }
        }
        Debug.Log("Scoreboard: Assigned player to Scoreboard Slot " + slotIndex);
    }

    public void ClearPlayerFromSlot(int slotIndex)
    {
        // iterate over all scoreboard slots and assign player to correct slot
        ScoreBoard_Player[] scoreboardSlots = GetComponentsInChildren<ScoreBoard_Player>();

        for (int i = 0; i < scoreboardSlots.Length; i++)
        {
            if (scoreboardSlots[i].SlotIndex == slotIndex)
            {
                scoreboardSlots[i].AssignedPlayer = null;
            }
        }
    }

    public void RemovePlayer(PhotonPlayer player)
    {
        // iterate over all scoreboard slots and assign player to correct slot
        ScoreBoard_Player[] scoreboardSlots = GetComponentsInChildren<ScoreBoard_Player>();

        for (int i = 0; i < scoreboardSlots.Length; i++)
        {
            if (scoreboardSlots[i].AssignedPlayer == player)
            {
                scoreboardSlots[i].AssignedPlayer = null;
            }
        }
    }

    public void RefreshPlayerSlots()
    {
        // iterate over all scoreboard slots and assign player to correct slot
        ScoreBoard_Player[] scoreboardSlots = GetComponentsInChildren<ScoreBoard_Player>();

        for (int i = 0; i < scoreboardSlots.Length; i++)
        {
            if (scoreboardSlots[i].AssignedPlayer == null)
            {
                
            }
        }
    }

    public void ToggleVisibility()
    {
        m_IsVisible = !m_IsVisible;
    }

    public void SetVisibility(bool isVisible)
    {
        m_IsVisible = isVisible;
    }
}
