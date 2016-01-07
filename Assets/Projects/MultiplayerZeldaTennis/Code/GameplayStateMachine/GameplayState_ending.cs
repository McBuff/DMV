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

            // freeze all objects & players
            

            // Camera goes back to countdown
            CameraPresetData presetData = CameraManager.GetInstance().GetCameraPresetPosition("CameraPreset_Countdown");
            CameraManager.GetInstance().LerpTo(presetData.CameraPos, presetData.CameraTarget);

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
