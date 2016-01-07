using UnityEngine;
using System.Collections;

public class Player_Weapon_HitBox : Player_Weapon {

    private MeshRenderer HitBoxObjectMeshRenderer;
	// Use this for initialization
	void Start () {
        m_MyCollider = GetComponentInChildren<Collider>();
        HitBoxObjectMeshRenderer = GetComponentInChildren<MeshRenderer>();
    }
	
	// Update is called once per frame
	void Update () {

        
        if (m_MyCollider == null)
        {
            Debug.LogWarning("Attacking with HITBOX weapon, without a hitbox");
            return;
        }

        if (isAttacking())
        {
            //TODO: play animation ( once I have a model )
            HitBoxObjectMeshRenderer.enabled = true;
            m_MyCollider.enabled = true;

        }
        else
        {
            m_MyCollider.enabled = false;
            HitBoxObjectMeshRenderer.enabled = false;
        }
        
    }


}
