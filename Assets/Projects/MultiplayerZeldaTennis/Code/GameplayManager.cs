using UnityEngine;
using System.Collections;
using Toolbox;
using DMV.GameplaystateManager;

/// <summary>
/// Manages Game states and enforces game rules. Nothing more
/// </summary>
[RequireComponent (typeof (PhotonView))]
public class GameplayManager : Photon.MonoBehaviour {

    // Enums 
    public GameplayStateType CurrentStateType;
    private GameplayStateType m_NextStateType;

    // State Object
    private GameplayState m_CurrentState;
    
    [System.Obsolete]
    public bool debugMode = false;
    
	// Use this for initialization
	void Start () {
        
        // Initialise base state
        CurrentStateType = GameplayStateType.waiting;
        m_NextStateType = GameplayStateType.waiting; 

        m_CurrentState = new GameplayState_waiting(this , PhotonNetwork.time);

        m_CurrentState.Init();
   } 
	
	// Update is called once per frame
	void Update () {
        
        // if I'm the server, regulate gameplay events. 
        if (PhotonNetwork.isMasterClient ) {

            // if a new state has been assigned by the state machine
            // create and init new state, send message to clients to change states
            if (m_NextStateType != m_CurrentState.GetCurrentStateType())
            {
                Debug.Log("Server says: Changing state!");
                photonView.RPC("SetGameplayState", PhotonTargets.Others, new object[] { (int)m_NextStateType, PhotonNetwork.time });
                SetGameplayState((int)m_NextStateType, PhotonNetwork.time);
            }            
        }
        // update correct state
        m_CurrentState.Update();

    }

    void OnGUI()
    {
        //TODO: Remove this
        GUILayout.Box(m_CurrentState.GetCurrentStateType().ToString());
    }

    /// <summary>
    /// Loads a new gameplay state, this state still needs to be initiated with "Init()"
    /// </summary>
    /// <param name="newState"></param>
    /// <param name="stateStartTime"></param>
    [PunRPC]
    public void SetGameplayState(int newState, double stateStartTime) {

        GameplayStateType NewState = (GameplayStateType)newState;

        switch (NewState)
        {
            case GameplayStateType.waiting:
                m_CurrentState = new GameplayState_waiting(this, stateStartTime);
                break;
            case GameplayStateType.countdown:
                m_CurrentState = new GameplayState_countdown(this, stateStartTime);
                break;
            case GameplayStateType.battle:
                m_CurrentState = new GameplayState_battle(this, stateStartTime);
                break;
            case GameplayStateType.ending:
                m_CurrentState = new GameplayState_ending(this, stateStartTime);
                break;
            default:
                break;
        }

        m_CurrentState.Init();

    }

    /// <summary>
    /// Sets up the stage for the NEXT gameplay state, note that this is only relevant for the SERVER
    /// </summary>
    /// <param name="type"></param>
    public void SetNextGameplayState( GameplayStateType type)
    {
        m_NextStateType = type;
    }
}

