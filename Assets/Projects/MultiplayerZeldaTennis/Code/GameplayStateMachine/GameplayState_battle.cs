using UnityEngine;
using System.Collections.Generic;


namespace DMV.GameplaystateManager
{
    class GameplayState_battle: GameplayState
    {
        protected List<Player> m_PlayerList;
        public GameplayState_battle(GameplayManager owner, double starttime) : base(owner, starttime)
        {
            
        }

        public override void Init()
        {
            // init... some... things???
            
            // Set all player positions to their respective spawn positions and enable movement
            PlayerManager.GetInstance().SpawnPlayers_All(); // I respawn everyone

            // Make sure ot Unfreeze all players
            PlayerManager.GetInstance().SetPlayerFrozen_All(false);
            // Server owner spawns a deathball in the middle of the room
            if (PhotonNetwork.player.isMasterClient)
            {
                SpawnDeathBall();
            }
                

            // Set Camera to follow player
            Player localplayer = PlayerManager.GetInstance().GetLocalPlayerObject();
            CameraManager.GetInstance().LerpFollow(localplayer.transform);

            // TODO: Start music


            m_PlayerList = PlayerManager.GetInstance().GetAllPlayerObjects();

            base.Init();
        }

        public override void Update()
        {

            // as soon as numplayers reaches 1, call game end and move to state ending!
            if (PhotonNetwork.isMasterClient)
            {
                List<Player> survivors = new List<Player>();
                for (int i = 0; i < m_PlayerList.Count; i++)
                {
                    Player plr = m_PlayerList[i];
                    if (plr != null)
                        survivors.Add(plr);
                }

                // annoucne next state
                if (survivors.Count == 1 && Input.GetKeyDown(KeyCode.F1 ))
                {
                    survivors[0].Photonplayer.AddScore(1);
                    AnnounceNextState(GameplayStateType.ending);
                }


            }

            

            
        }

        protected void SpawnDeathBall()
        {
            /*GameObject newObject =*/ PhotonNetwork.Instantiate("DeathBallv2", Vector3.up * .5f, Quaternion.identity, 0); // TODO: Use prefab.name

        }
        public override GameplayStateType GetCurrentStateType()
        {
            return GameplayStateType.battle;
        }

    }
}
