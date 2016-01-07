using UnityEngine;
using System.Collections;

public class Player_Weapon_Swipe : Player_Weapon {

    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        MeshRenderer bMesh = GetComponentInChildren<MeshRenderer>();
        BoxCollider bCol = GetComponentInChildren<BoxCollider>();


        // if attack is not yet finished, do attack
        if(isAttacking())
        {
            bCol.enabled = true;
            bMesh.enabled = true;


            // attack code
            float attackProgress = GetAttackProgress();
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

    public override void Attack(float attackdirection, float starttime = 0) {
        if (starttime == 0)
            AttackStartTime = Time.time;
        else AttackStartTime = starttime;

        m_PlayerAttackDirection = attackdirection;
    }
}
