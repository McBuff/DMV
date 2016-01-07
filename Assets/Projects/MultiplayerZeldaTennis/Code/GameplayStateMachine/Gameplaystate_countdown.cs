using UnityEngine;
using System.Collections.Generic;
using Toolbox;
using System;

namespace DMV.GameplaystateManager
{
    class GameplayState_countdown : GameplayState
    {
        // Timer Used to go to next state
        private Local_Timer m_Timer;

        public GameplayState_countdown(GameplayManager owner, double starttime) : base(owner, starttime)
        {

        }

        public override void Init()
        {
            // Create a new 3 second timer taking in account for lost time ( network or after announce )
            m_Timer = new Local_Timer();            
            double timeLost = PhotonNetwork.time - m_StateStartTime; // time passed between announce & actual handling
            m_Timer.StartTimer(Time.time + 3, Time.time - timeLost);
            

            // Create the visual timer Object, this one is just for show
            UnityEngine.Object CountDownTextObject = (GameObject)Resources.Load("CountDownText");
            GameObject newTimerObject = (GameObject)GameObject.Instantiate(CountDownTextObject, Vector3.zero, Quaternion.identity);
            newTimerObject.transform.Rotate(Vector3.right, 90f);
            newTimerObject.name = "Snackbar";
            newTimerObject.GetComponent<countdownText>().StartCountdown(Time.time + 4, Time.time);

            
            PlayerManager.GetInstance().SpawnPlayers_All();

            // Freeze all players
            PlayerManager.GetInstance().SetPlayerFrozen_All(true, false);

                        // Move Camera to Countdown position
            CameraPresetData presetData = CameraManager.GetInstance().GetCameraPresetPosition("CameraPreset_Countdown");
            CameraManager.GetInstance().LerpTo(presetData.CameraPos, presetData.CameraTarget);

            // Hide scoreboard
            ScoreBoard.GetInstance().SetVisibility(false);


            #region old code
            /*
            // OLD CODE

            // Set all player positions to their respective spawn positions
            //GameObject playerList = GameObject.Find("Players");
            //GameObject spawnList = GameObject.Find("Spawners");
            //for (int i = 0; i < playerList.transform.childCount; i++)
            //{
            //    Transform playerTransform = playerList.transform.GetChild(i);

            //    Player player = playerTransform.GetComponent<Player>();
            //    Player_Movement player_movement = playerTransform.GetComponent<Player_Movement>();
            //    Transform spawnpoint = spawnList.transform.GetChild(i).transform;

            //    player.transform.position = spawnpoint.position;
            //    player_movement.enabled = false;
            //}
            */
            #endregion

            base.Init();
        }

        public override void Update()
        {

            // Freeze all players

            if (m_Timer != null)
            {
                m_Timer.updateTimer();


                if (m_Timer.isFinished)
                    AnnounceNextState(GameplayStateType.battle);

            }

            //Manual skip ( server side )
            if (Input.GetKeyDown(KeyCode.F1))
            {
                // announce next state
                if (PhotonNetwork.isMasterClient)
                    AnnounceNextState(GameplayStateType.battle);
            }


            
        }

        public override GameplayStateType GetCurrentStateType()
        {
            return GameplayStateType.countdown;
        }

    }
}