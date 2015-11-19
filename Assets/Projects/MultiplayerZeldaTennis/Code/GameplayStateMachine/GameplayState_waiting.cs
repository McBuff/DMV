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

            // make sure the level is clean maybe, I dunno
            base.Init();
        }

        public override void Update()
        {
            
            if ( m_Owner.debugMode && Input.GetKeyDown(KeyCode.F1))
            {
                // announce next state
                Debug.LogWarning("Setnextstate!");
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
