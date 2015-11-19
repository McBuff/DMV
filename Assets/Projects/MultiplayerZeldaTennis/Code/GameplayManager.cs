using UnityEngine;
using System.Collections;
using Toolbox;
using DMV.GameplaystateManager;

/// <summary>
/// Manages Game states and enforces game rules. Nothing more
/// </summary>
[RequireComponent (typeof (PhotonView))]
public class GameplayManager : Photon.MonoBehaviour {



    public GameplayStateType CurrentStateType;

    private GameplayStateType m_NextStateType;
    private GameplayState m_CurrentState;
    

    public bool debugMode = false;
    private Photon_Timer countDownTimer; 
	// Use this for initialization
	void Start () {
        CurrentStateType = GameplayStateType.waiting;
        m_NextStateType = GameplayStateType.waiting; 

        m_CurrentState = new GameplayState_waiting(this , PhotonNetwork.time);
        m_CurrentState.Init();
   } 
	
	// Update is called once per frame
	void Update () {
        
        

        // if I'm the server, regulate gameplay events. 
        if (PhotonNetwork.isMasterClient ) {

            // create and init new state, send message to clients to change states
            if (m_NextStateType != m_CurrentState.GetCurrentStateType())
            {
                Debug.LogWarning("Changing state!");
                photonView.RPC("SetGameplayState", PhotonTargets.Others, new object[] { (int)m_NextStateType, PhotonNetwork.time });
                SetGameplayState((int)m_NextStateType, PhotonNetwork.time);
            }
            
        }
        // update correct state
        m_CurrentState.Update();

    }

    void OnGUI()
    {

        GUILayout.Box("");
        GUILayout.Box(m_CurrentState.GetCurrentStateType().ToString());
    }

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

    public void SetNextGameplayState( GameplayStateType type)
    {
        m_NextStateType = type;
    }
}

