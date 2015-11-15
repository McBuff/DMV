using UnityEngine;
using System.Collections;

/// <summary>
/// Manages Game states
/// </summary>
public class GameplayManager : Photon.MonoBehaviour {

    public GamePlayState CurrentState;

	// Use this for initialization
	void Start () {
        CurrentState = GamePlayState.waiting;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    [PunRPC]
    void SetGameplayState(GamePlayState newState, double photonTime) {

        if (PhotonNetwork.isMasterClient)
        {
            if(newState == GamePlayState.countdown && CurrentState != newState)
            {

                // spawn players, spawn deathball
            }
        }
        else {

        }
    }
}

public enum GamePlayState {
    waiting,
    countdown,
    battle,
    ending
}