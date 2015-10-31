using UnityEngine;
using System.Collections;

public class Controller_FPS_Upperbody : MonoBehaviour {

    public Transform PlayerCam;
    public Transform PlayerBody;

	// Use this for initialization
	void Start () {

        if (PlayerBody == null)
            throw new System.Exception("Weapon/upperbody has no camera attached");
	}
	
	// Update is called once per frame
	void Update () {

        // rotate over X axis to reach mouseaim

        transform.rotation = PlayerCam.transform.rotation;
        
        // todo make upper body both Y and X rotatable, let the lower body follow


    }
}
