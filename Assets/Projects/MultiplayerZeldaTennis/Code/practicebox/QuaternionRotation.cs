using UnityEngine;
using System.Collections;

public class QuaternionRotation : MonoBehaviour {

    public Quaternion Rotation;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        transform.rotation = transform.rotation* Rotation;
        

	}
}
