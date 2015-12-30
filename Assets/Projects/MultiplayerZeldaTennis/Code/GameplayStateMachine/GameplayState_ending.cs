using UnityEngine;
using System.Collections;
using Toolbox;
using System;

namespace DMV.GameplaystateManager
{
    class GameplayState_ending : GameplayState
    {

        public GameplayState_ending(GameplayManager owner, double starttime) : base(owner, starttime)
        {

        }

        public override void Init()
        {
            // init... some... things???

            // make sure the level is clean maybe, I dunno

            // Camera goes back to countdown

            base.Init();
        }

        public override void Update()
        {
                                   
            
        }

        public override GameplayStateType GetCurrentStateType()
        {
            return GameplayStateType.ending;
        }

    }
}
