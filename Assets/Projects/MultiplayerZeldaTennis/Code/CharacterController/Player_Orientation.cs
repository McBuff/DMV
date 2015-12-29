using UnityEngine;
using System.Collections;

public class Player_Orientation : Photon.MonoBehaviour
{
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        // Owner Logic
        if (photonView.isMine)
            update_owner();

        // Follower
        else update_other();

    }

    // Updates internal logic as owner of object (player)
    void update_owner() {

    }

    // Simulates internal logic using received data
    void update_other() {

    }
}
