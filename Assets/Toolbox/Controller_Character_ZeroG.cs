using UnityEngine;
using System.Collections;

/// <summary>
///  In zeroG, a character orients using its own transform
/// </summary>
public class Controller_Character_ZeroG : MonoBehaviour {
    
    public Camera Cam;
    

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

        

        UpdateZeroG();

    }

    private void UpdateZeroG() {
        // I have control over the Y axis AND the walking plane ( normalized vector )

        // basic controls
        float Speed = 4.0f;
        Vector3 LocalMovement = new Vector3();
        // Forward, Backward
        if (Input.GetKey(KeyCode.W))
            LocalMovement += Vector3.forward * Speed * Time.deltaTime;
         
        if (Input.GetKey(KeyCode.S))
            LocalMovement = Vector3.back * Speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
            LocalMovement += Vector3.left * Speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.D))
            LocalMovement += Vector3.right * Speed * Time.deltaTime;

        //Vector3 WorldMovement = transform.localToWorldMatrix * LocalMovement;
        transform.Translate(LocalMovement);

        // rotation controls
        //transform.Rotate(Vector3.up, 0.5f);

        //transform.rotation = Quaternion.Lerp(Cam.transform.rotation, transform.rotation, 0.95f);
        //transform.LookAt(transform.position + transform.forward, Vector3.up); // force camera upright

    }
}
