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

            // make sure the level is clean maybe, I dunno


            // Set all player positions to their respective spawn positions and enable movement
            PlayerManager.GetInstance().SpawnPlayers_All(); // I respawn everyone


            // Set Camera to follow player
            Player localplayer = PlayerManager.GetInstance().GetLocalPlayerObject();
            CameraManager.GetInstance().LerpFollow(localplayer.transform);

            // Todo: Start music

            base.Init();
        }

        public override void Update()
        {
                                   
            
        }

        public override GameplayStateType GetCurrentStateType()
        {
            return GameplayStateType.battle;
        }

    }
}
