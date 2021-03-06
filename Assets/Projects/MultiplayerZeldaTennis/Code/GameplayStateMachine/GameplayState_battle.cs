﻿using UnityEngine;
using System.Collections.Generic;
using Player;

namespace DMV.GameplaystateManager
{
    class GameplayState_battle: GameplayState
    {
        protected List<Player.PlayerController> m_PlayerList;
        public GameplayState_battle(GameplayManager owner, double starttime) : base(owner, starttime)
        {
            
        }

        public override void Init()
        {
            // init... some... things???
            
            // Set all player positions to their respective spawn positions and enable movement
            PlayerManager.GetInstance().SpawnPlayers_All(); // I respawn everyone

            // Make sure to UnRoot all players ( cleanse players of anything anyway )
            PlayerManager.GetInstance().RemovePlayerConditions_All();
            
            // Server owner spawns a deathball in the middle of the room
            if (PhotonNetwork.player.isMasterClient)
            {
                SpawnDeathBall();
            }
                

            // Set Camera to follow player
            PlayerController localplayer = PlayerManager.GetInstance().GetLocalPlayerObject();
            CameraManager.GetInstance().LerpFollow(localplayer.transform);

            // TODO: Start music


            m_PlayerList = PlayerManager.GetInstance().GetAllPlayerObjects();

            base.Init();
        }

        public override void Update()
        {
         
            if(isStateFinished())
            {
                AnnounceNextState(GameplayStateType.ending);
            }
            
        }


        public override bool isStateFinished()
        {
            // contains debug info
            bool baseResult = base.isStateFinished();
            bool stateResult = false;

            // as soon as numplayers reaches 1, call game end and move to state ending!
            if (PhotonNetwork.isMasterClient)
            {
                List<PlayerController> survivors = new List<PlayerController>();
                for (int i = 0; i < m_PlayerList.Count; i++)
                {
                    PlayerController plr = m_PlayerList[i];
                    if (plr != null)
                        survivors.Add(plr);
                }

                // annoucne next state 
                if (survivors.Count == 1)
                {
                    survivors[0].Photonplayer.AddScore(1);
                    stateResult = true;
                }
                if (survivors.Count == 0)
                {
                    stateResult = true;
                }

            }
            return (baseResult && stateResult); // only if base and state are true, shall I progress to the enxt state

        }

        /// <summary>
        /// Spawns a deathball at the default position
        /// </summary>
        protected void SpawnDeathBall()
        {
            m_Owner.DeathBallInstance = PhotonNetwork.Instantiate("DeathBallv2", Vector3.up * .5f, Quaternion.identity, 0).GetComponent<BouncingProjectile>(); // TODO: Use prefab.name 
        }


        public override GameplayStateType GetCurrentStateType()
        {
            return GameplayStateType.battle;
        }

    }
}
