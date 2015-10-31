using UnityEngine;
using System.Collections;

public class Camera_FirstPerson : MonoBehaviour {

    public Vector2 LastMousePos;
    public float xSensitiviy = 1.5f;
    public float ySensitiviy = 1.5f;
    public bool LockMouse = false;


    /// <summary>
    ///  The body this camera is attached to
    /// </summary>
    /// 
    public Transform Body;

    // Use this for initialization
    void Start () {
	    LastMousePos = Input.mousePosition;

        if (Body == null)
            throw new System.Exception("Camera FirstPerson has no Body");
    }
	
	// Update is called once per frame
	void Update () {
        PositionCamera();

        // handle mouse lock ( simplified )
        if (LockMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            LockMouse = !LockMouse;

        // get mouse distance ( as an Axis )
        Vector3 mouseOffset = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f);
        mouseOffset = new Vector3(xSensitiviy * mouseOffset.x, ySensitiviy * mouseOffset.y, mouseOffset.z); // mouse sensitivy

        
        Quaternion targetRotation = new Quaternion(mouseOffset.x * Time.deltaTime, mouseOffset.y *Time.deltaTime, mouseOffset.z, 1);
        Quaternion currentRotation = transform.rotation;
        // what if transformations were lerped?


        Quaternion actualRotation = Quaternion.Lerp(new Quaternion(), targetRotation, 0.1f);

        transform.rotation *= actualRotation;

        
        //transform.Rotate(mouseOffset, Space.Self); //rotates the camera
        transform.LookAt(transform.position + transform.forward, Body.up); // force camera upright
        
        // finish
        //LastMousePos = currentMousePos;

	}

    /// <summary>
    /// Places the camera inside of the Body at 0.95f height ( from center )
    /// </summary>
    private void PositionCamera() {

        transform.position = Body.position;
        transform.Translate(Vector3.up * 0.95f, Body.transform);

    }
}
