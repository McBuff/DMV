using UnityEngine;
using System.Collections;

public class Camera_Controller : MonoBehaviour {

    public Transform targetPlayer;

    public Vector3 CameraOffset;
    public Vector3 CameraLookatOffset;

    private float cameraSpeed;
	// Use this for initialization
	void Start () {
        CameraOffset = new Vector3(0, 15.0f, 0);
        cameraSpeed = .2f;
    }
	
	// Update is called once per frame
	void Update () {

        if( targetPlayer != null) { 
        // interpolate camera pos with previous pos
        Vector3 currentpos, nextpos;
                
        currentpos = transform.position;
        nextpos = targetPlayer.position + CameraOffset;

        nextpos = Vector3.Lerp(currentpos , nextpos , cameraSpeed);

        transform.position = nextpos;
        }

    }
}
