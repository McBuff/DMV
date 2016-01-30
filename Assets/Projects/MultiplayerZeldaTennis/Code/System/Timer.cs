using UnityEngine;
using System.Collections;
using System;


namespace Toolbox
{
    /// <summary>
    /// Simple timer class that checks if a time has expired, does not fire events. only updates when checked
    /// </summary>
    class Photon_Timer
    {
        private double startTime;
        private double endTime;
        private bool isRunning;


        // C-Tor
        public Photon_Timer()
        {

        }

        public bool IsRunning() {
            return isRunning;
        }


        /// <summary>
        /// Start the timer, with a given endtime
        /// </summary>
        /// <param name="endTime"></param>
        public void StartTimer(double endtime, double startTime_override = 0) {
            if (startTime == 0)
                startTime = PhotonNetwork.time;
            else startTime = startTime_override;

            if (PhotonNetwork.time == 0)
            {
                startTime = Time.time;
                endTime = Time.time + endtime;
            }

            endTime = endtime;
            isRunning = true;
        }
        /// <summary>
        /// Checks the clock for expiration, if finished, yields true
        /// </summary>
        /// <returns></returns>
        public bool isTimerFinished()
        {

            double currentTime = PhotonNetwork.time;
            if (PhotonNetwork.time == 0)
                currentTime = Time.time;

            int clockStatus = currentTime.CompareTo(endTime);
            if (isRunning && clockStatus == 1) {
                isRunning = false;
                return true;
            }

            return false;
        }

        public double GetTimeLeft() {

            if (isRunning)
            {
                if (PhotonNetwork.time.Equals(0) == false)
                    return endTime - PhotonNetwork.time;
                return endTime - Time.time;

            }
            return 0;
        }

    }


    class Local_Timer{

        public bool isRunning = false;
        public bool isFinished = false;
        private double m_endTime;
        private double m_startTime;
        public Local_Timer() { }


        public int GetSecondsLeft()
        {
            return (int)Math.Truncate(GetTimeleft());
        }

        /// <summary>
        /// Starts timer
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="startTime"></param>
        public void StartTimer( double endTime, double startTime = -1)
        {
            m_startTime = startTime;

            if (m_startTime == -1)
                m_startTime = Time.time;

            
            m_endTime = endTime;
            isFinished = false;
            isRunning = true;
            //Debug.LogWarning("Timer set for: " + m_endTime);
        }

        public void updateTimer()
        {
            //Debug.Log("updateTimer: " + m_endTime);
            // check if timer has expired
            if(Time.time > m_endTime && isRunning)
            {
                isRunning = false;
                isFinished = true;

            }

        }

        public double GetTimeleft() {
            return m_endTime - Time.time;
        }



    }
}
