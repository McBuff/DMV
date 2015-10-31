using UnityEngine;
using System.Collections;

public class Player_Weapon : MonoBehaviour {

    private float Angle = 90f;
    private float Duration = .05f;

    private float startTime = 0;
    private float startAngle = -45f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        MeshRenderer bMesh = GetComponentInChildren<MeshRenderer>();
        BoxCollider bCol = GetComponentInChildren<BoxCollider>();

        if (Input.GetMouseButtonDown(0))
        {
            // do swipe?
            startTime = Time.time;
        }

        // if attack is not yet finished, do attack
        float endTime = Duration + startTime;
        if( Time.time <= endTime && startTime != 0)
        {
            bCol.enabled = true;
            bMesh.enabled = true;


            // attack code
            float attackProgress = (Time.time - startTime) / (endTime - startTime);
            float currentAngle = Mathf.LerpAngle(-45 , 45 , attackProgress);
            transform.rotation = Quaternion.Euler( 0, currentAngle, 0);

            
        }
        else
        {
            bCol.enabled = false;
            bMesh.enabled = false;

        }



	}
}
