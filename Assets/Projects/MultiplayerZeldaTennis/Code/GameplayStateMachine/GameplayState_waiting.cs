using UnityEngine;
using System.Collections;
using Toolbox;
using System;
using Player;

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
            PlayerManager.GetInstance().RemovePlayerConditions_All();
            
            //PlayerManager.GetInstance().SetPlayerFrozen_All(false, false);


            base.Init();
        }

        /// <summary>
        /// Update logic
        /// </summary>
        public override void Update()
        {            

            if (isStateFinished())
            {                
                AnnounceNextState(GameplayStateType.countdown);
            }

        }

        public override bool isStateFinished()
        {
            int numConnectedPlayers = PhotonNetwork.playerList.Length;
            bool baseState = base.isStateFinished();
            bool waitingState = (numConnectedPlayers > 1);

            return baseState || waitingState;
        }

        public override GameplayStateType GetCurrentStateType()
        {
            return GameplayStateType.waiting;
        }

    }
}
