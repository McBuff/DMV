using UnityEngine;
using System.Collections;
using ManagedSteam;
using ManagedSteam.SteamTypes;

public class SteamControllerHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        SteamControllerState steamControllerState;

        Debug.Log(Steamworks.SteamInterface);
        Steamworks.SteamInterface.SteamController.GetControllerState(0, out steamControllerState);
        //Steamworks.SteamInterface.SteamController.TriggerHapticPulse(0, ManagedSteam.SteamTypes.SteamControllerPad.Left, 1);
	}
}
