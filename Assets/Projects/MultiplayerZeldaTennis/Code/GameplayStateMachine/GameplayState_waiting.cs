using UnityEngine;
using System.Collections;
using Toolbox;
using System;

namespace DMV.GameplaystateManager
{
    class GameplayState_waiting: GameplayState
    {

        public GameplayState_waiting(GameplayManager owner, double starttime) : base(owner, starttime)
        {

        }

        public override void Init()
        {
            // init... some... things???


            // Show scoreboard
            ScoreBoard.GetInstance().SetVisibility(false);

            // make sure the level is clean maybe, I dunno

            // clear up all projectiles 
            if (m_Owner.DeathBallInstance != null )
            {
                // destroy 
                if (PhotonNetwork.isMasterClient)
                    PhotonNetwork.Destroy(m_Owner.DeathBallInstance.gameObject);
            }

            // Freeze all players
            PlayerManager.GetInstance().SetPlayerFrozen_All(false, false);


            base.Init();
        }

        public override void Update()
        {
            
            if ( Input.GetKeyDown(KeyCode.F1))
            {
                // announce next state
                Debug.Log("Manually Advancing to countdown state, server pressed F1");
                AnnounceNextState(GameplayStateType.countdown);
                //m_Owner.SetNextGameplayState();
            }

            int numConnectedPlayers = PhotonNetwork.playerList.Length;

            if (numConnectedPlayers > 1)
            {
                // announce next state
                Debug.Log("Automatically Advancing to countdown state, 2 players are present");
                AnnounceNextState(GameplayStateType.countdown);
                //m_Owner.SetNextGameplayState();
            }


        }

        public override GameplayStateType GetCurrentStateType()
        {
            return GameplayStateType.waiting;
        }

    }
}
