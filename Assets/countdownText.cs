using UnityEngine;
using System.Collections;
using Toolbox;
using System;

public class countdownText : MonoBehaviour {

    private Local_Timer m_Timer;
    // Use this for initialization
    void Start () {
        
        
    }
	
	// Update is called once per frame
	void Update () {

        
        if (m_Timer != null)
        {
            m_Timer.updateTimer();

            if (m_Timer.isFinished)
            {
                Destroy(gameObject);
            }

            if (m_Timer.isRunning)
            {
                double timeLeft = m_Timer.GetTimeleft();
                string timeLeftText = Mathf.Floor((float)timeLeft).ToString();
                if (timeLeftText == "0")
                    timeLeftText = "GO!";

                // update text in text component
                GetComponent<TextMesh>().text = timeLeftText;

            }
        }
        

    }


    public void StartCountdown(double endTime, double startTime = -1)
    {
        
        //Debug.LogWarning("Starting countdown: " + endTime + ", " + startTime);

        m_Timer = new Local_Timer();
        
        m_Timer.StartTimer(endTime);
    }
}
