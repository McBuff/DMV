using UnityEngine;
using System.Collections;

public class Controller_character_FPS_ZeroG : MonoBehaviour {

    public Transform PlayerCam;
    public Transform PlayerUpperBody;
    public Transform PlayerLowerBody;
        
    public bool LockMouse = true;

    private Vector3 WorldInertia;

    // Use this for initialization
    void Start () {

        WorldInertia = Vector3.zero;

        if ( PlayerCam == null) {
            throw new System.Exception("FPS Controller: Playercam is null");
        }
        if (PlayerUpperBody == null)
        {
            throw new System.Exception("FPS Controller: PlayerUpperBody is null");
        }
        if (PlayerLowerBody == null)
        {
            throw new System.Exception("FPS Controller: PlayerLowerBody is null");
        }

    }
	
	// Update is called once per frame
	void Update () {

        HandleMouseLock();

        // Cam
        // FOLLOWS:
        //      - MouseX, MouseY
        //      - Lowerbody Rotation
        float xSensitiviy = 2f;
        float ySensitiviy = 2f;
        // get mouse distance ( as an Axis )
        Vector3 mouseOffset = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f);
        mouseOffset = new Vector3(xSensitiviy * mouseOffset.x, ySensitiviy * mouseOffset.y, mouseOffset.z); // mouse sensitivy
        
        Quaternion targetRotation = new Quaternion(mouseOffset.x * Time.deltaTime, mouseOffset.y * Time.deltaTime, mouseOffset.z, 1);
        Quaternion currentRotation = PlayerLowerBody.rotation;
        PlayerCam.rotation *= targetRotation;
        PlayerCam.LookAt(PlayerCam.position + PlayerCam.forward, PlayerLowerBody.up); // force camera upright

        // set position
        PlayerCam.localPosition = PlayerLowerBody.localPosition + PlayerLowerBody.up * 1.4f; ;

        // Upperbody:
        //  FOLLOWS:
        //        - Y axis of mouse
        //        - X axis of mouse

        PlayerUpperBody.localPosition = PlayerLowerBody.localPosition + PlayerLowerBody.up * 0.8f;
        PlayerUpperBody.localEulerAngles = new Vector3(PlayerCam.localEulerAngles.x, PlayerCam.localEulerAngles.y, PlayerLowerBody.localEulerAngles.z);

        

        // lowerbody
        //  FOLLOWS:
        //         - Y rotation of Upperbody
        //  CONTROLS:
        //         - Z Axis  Roll
        //         - Movement X, Y , Z
        //         
        //PlayerLowerBody.RotateAround(Vector3.up, PlayerUpperBody.rotation.ToEuler().y);
        PlayerLowerBody.localEulerAngles = new Vector3(PlayerLowerBody.localEulerAngles.x, PlayerUpperBody.localEulerAngles.y, PlayerLowerBody.localEulerAngles.z);
        HandleLowerBodyMovement();
        HandleLowerBodyRoll();

        HandleMouseLook();
    }

    private void HandleMouseLook(){

    }
    private void HandleLowerBodyMovement()
    {
        // basic controls
        float Speed = 0.2f;
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

        if (Input.GetKey(KeyCode.Space))
            LocalMovement += Vector3.up * Speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftShift))
            LocalMovement += Vector3.down * Speed * Time.deltaTime;

        //Vector3 WorldMovement = transform.localToWorldMatrix * LocalMovement;
        WorldInertia += LocalMovement;

        WorldInertia = Vector3.ClampMagnitude(WorldInertia, 1.0f);

        PlayerLowerBody.Translate(WorldInertia);

        WorldInertia *= 0.99f;
    }
    private void HandleLowerBodyRoll() {

        if (Input.GetKey(KeyCode.Q)) 
            PlayerLowerBody.Rotate(Vector3.forward, 40.0f * Time.deltaTime);

        if (Input.GetKey(KeyCode.E)) 
                PlayerLowerBody.Rotate(Vector3.forward, -40.0f * Time.deltaTime);
        
    }

    private void HandleMouseLock()
    {
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
    }
}
