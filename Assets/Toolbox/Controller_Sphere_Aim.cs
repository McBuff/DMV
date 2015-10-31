using UnityEngine;
using System.Collections;

public class Controller_Sphere_Aim : MonoBehaviour {


    public bool LockMouse = true;

    public Transform XAxis;
    public Transform YAxis;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        float xSensitiviy = 2f;
        float ySensitiviy = 2f;
        // get mouse distance ( as an Axis )
        Vector3 mouseOffset = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f);
        mouseOffset = new Vector3(xSensitiviy * mouseOffset.x, ySensitiviy * mouseOffset.y, mouseOffset.z); // mouse sensitivy

        Quaternion mouseRotation = new Quaternion(mouseOffset.x * Time.deltaTime, mouseOffset.y * Time.deltaTime, mouseOffset.z, 1);


        Vector3 xMouse = new Vector3(mouseOffset.x, 0, 0);
        Vector3 yMouse = new Vector3(0, mouseOffset.y, 0);

        XAxis.Rotate(xMouse, Space.Self);
        YAxis.Rotate(yMouse, Space.Self);
        //Quaternion xRot = new Quaternion(Mathf.Sin( mouseOffset.x * 0.1f), 0, 0, 1);
        //Quaternion yRot = new Quaternion(0, Mathf.Sin ( mouseOffset.y *.1f ), 0, 1);

        //XAxis.rotation *= xRot;
        //YAxis.rotation *= yRot;

        Quaternion upDir = Quaternion.FromToRotation(transform.forward, transform.up);
        float Angle = Quaternion.Angle(upDir, transform.rotation* mouseRotation);

        //transform.rotation *= mouseRotation;
        Debug.Log(Angle);
           
        //transform.LookAt(transform.position + transform.forward, Vector3.up); // force camera upright
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
