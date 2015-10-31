using UnityEngine;
using System.Collections;

public class LookAtDirection : MonoBehaviour {

    Vector3 lastPos;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 currentPos = transform.position;

        Vector3 direction = (currentPos - lastPos).normalized;

        transform.LookAt(transform.position + direction, transform.up);
        lastPos = transform.position;

        transform.Rotate(Vector3.up,90.0f, Space.World);
    }
}
