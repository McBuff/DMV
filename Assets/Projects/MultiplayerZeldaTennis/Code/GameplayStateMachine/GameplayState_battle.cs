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
            GameObject playerList = GameObject.Find("Players");
            GameObject spawnList = GameObject.Find("Spawners");
            for (int i = 0; i < playerList.transform.childCount; i++)
            {
                Transform playerTransform = playerList.transform.GetChild(i);

                Player player = playerTransform.GetComponent<Player>();
                Player_Movement player_movement = playerTransform.GetComponent<Player_Movement>();
                Transform spawnpoint = spawnList.transform.GetChild(i).transform;

                player.transform.position = spawnpoint.position;
                player_movement.enabled = true;
            }


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
