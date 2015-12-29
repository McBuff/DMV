using UnityEngine;
using System.Collections;

public class Player_Weapon : MonoBehaviour {

    
    private float Duration = .05f;

    private float startTime = 0;
    

    private float m_PlayerAttackDirection;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        MeshRenderer bMesh = GetComponentInChildren<MeshRenderer>();
        BoxCollider bCol = GetComponentInChildren<BoxCollider>();


        // if attack is not yet finished, do attack
        float endTime = Duration + startTime;
        if( Time.time <= endTime && startTime != 0)
        {
            bCol.enabled = true;
            bMesh.enabled = true;


            // attack code
            float attackProgress = (Time.time - startTime) / (endTime - startTime);
            float currentAngle = Mathf.LerpAngle(-45 , 45 , attackProgress);
            transform.rotation = Quaternion.Euler( 0, m_PlayerAttackDirection + currentAngle, 0);
            
        }
        else
        {
            bCol.enabled = false;
            bMesh.enabled = false;
            //gameObject.SetActive(false);
        }



	}

    public void Attack(float attackdirection, float starttime = 0) {
        if (starttime == 0)
            startTime = Time.time;
        else startTime = starttime;

        m_PlayerAttackDirection = attackdirection;
    }
}
