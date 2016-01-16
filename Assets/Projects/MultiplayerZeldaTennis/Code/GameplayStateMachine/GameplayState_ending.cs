//#define DEBUG

using UnityEngine;
using System.Collections.Generic;
using Toolbox;
using System;



namespace DMV.GameplaystateManager
{
    class GameplayState_ending : GameplayState
    {

        // Timer Used to go to next state
        private Local_Timer m_Timer;


        // Let's try this for once
        List<string> m_CountdownTextBuffer;

        public GameplayState_ending(GameplayManager owner, double starttime) : base(owner, starttime)
        {
            

        }

        public override void Init()
        {
            // This state lasts for 5 seconds?
            // Create a new 3 second timer taking in account for lost time ( network or after announce )
            m_Timer = new Local_Timer();
            double timeLost = PhotonNetwork.time - m_StateStartTime; // time passed between announce & actual handling
            m_Timer.StartTimer(Time.time + 6, Time.time - timeLost);


            // freeze all objects & players
            PlayerManager.GetInstance().SetPlayerFrozen_All(true,true);
            // is there a deahball?
            if(m_Owner.DeathBallInstance != null)
                m_Owner.DeathBallInstance.MovementSpeed = 0;
            
            // Camera goes back to countdown
            CameraPresetData presetData = CameraManager.GetInstance().GetCameraPresetPosition("CameraPreset_Countdown");
            CameraManager.GetInstance().LerpTo(presetData.CameraPos, presetData.CameraTarget);

            // Show scoreboard
            ScoreBoard.GetInstance().SetVisibility(true);


            m_CountdownTextBuffer = new List<string>();

            m_CountdownTextBuffer.Add("<b>Resetting</b>");
            m_CountdownTextBuffer.Add("<b>Game will reset in 1 seconds...</b>");
            m_CountdownTextBuffer.Add("<b>Game will reset in 2 seconds...</b>");
            m_CountdownTextBuffer.Add("<b>Game will reset in 3 seconds...</b>");
            m_CountdownTextBuffer.Add("<b>Game will reset in 4 seconds...</b>");


            base.Init();
        }

        public override void Update()
        {
            if (m_Timer != null)
            {
                m_Timer.updateTimer();

                // Log message every second that passes? How can I see if a message has not been added yet


                int secondsLeft = m_Timer.GetSecondsLeft();

                #if(  DEBUG )
                Debug.Log("Seconds left: " + secondsLeft + "countdownbufferLength: " + m_CountdownTextBuffer.Count);
                #endif 


                if( m_CountdownTextBuffer.Count - 1 == secondsLeft)
                {
                    // Print LAST item, and remove it
                    EventLog.GetInstance().LogMessage(m_CountdownTextBuffer[m_CountdownTextBuffer.Count - 1]);
                    m_CountdownTextBuffer.RemoveAt(m_CountdownTextBuffer.Count - 1);

                }

                if (m_Timer.isFinished)
                    AnnounceNextState(GameplayStateType.waiting);

            }

        }

        public override GameplayStateType GetCurrentStateType()
        {
            return GameplayStateType.ending;
        }

    }
}
