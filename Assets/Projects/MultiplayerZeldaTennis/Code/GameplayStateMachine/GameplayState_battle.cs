using UnityEngine;
using System.Collections;
using Toolbox;
using System;

namespace DMV.GameplaystateManager
{
    class GameplayState_battle: GameplayState
    {

        public GameplayState_battle(GameplayManager owner, double starttime) : base(owner, starttime)
        {

        }

        public override void Init()
        {
            // init... some... things???
            
            // Set all player positions to their respective spawn positions and enable movement
            PlayerManager.GetInstance().SpawnPlayers_All(); // I respawn everyone

            // Server owner spawns a deathball in the middle of the room
            if (PhotonNetwork.player.isMasterClient)
            {
                SpawnDeathBall();
            }
                

            // Set Camera to follow player
            Player localplayer = PlayerManager.GetInstance().GetLocalPlayerObject();
            CameraManager.GetInstance().LerpFollow(localplayer.transform);

            // Todo: Start music

            base.Init();
        }

        public override void Update()
        {
                                   
            
        }

        protected void SpawnDeathBall()
        {
            GameObject newObject = PhotonNetwork.Instantiate("DeathBallv2", Vector3.up * .5f, Quaternion.identity, 0); // TODO: Use prefab.name

        }
        public override GameplayStateType GetCurrentStateType()
        {
            return GameplayStateType.battle;
        }

    }
}
