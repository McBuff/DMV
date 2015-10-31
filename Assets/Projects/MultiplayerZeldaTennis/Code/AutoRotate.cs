using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {

    public Vector3 RotationAxis;
    public float RotationSpeed = 100.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        transform.Rotate(RotationAxis, RotationSpeed * Time.deltaTime);
        
    }
}
