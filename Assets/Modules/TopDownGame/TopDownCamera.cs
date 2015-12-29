using UnityEngine;
using System.Collections;

public class TopDownCamera : MonoBehaviour {

    public Transform PrimaryTarget;

    public enum CameraState
    {
        Default,
        Manual,
        Error
    }

    public CameraState m_CameraState = CameraState.Manual;

	// Use this for initialization
	void Start () {
	
        

	}
	
	// Update is called once per frame
	void Update () {

        // output debug info
        OutputDebugInfo();
    }

    void OutputDebugInfo() {

        // Generate a debug report
        string debugReport = "DebugReport ";
        debugReport += transform.name + ": ";

        debugReport += "Camera Mode: " + m_CameraState;  // report on camera m ode

        Debug.Log(debugReport);
    }
}
