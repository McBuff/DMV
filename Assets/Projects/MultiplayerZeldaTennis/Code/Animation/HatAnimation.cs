using UnityEngine;
using System.Collections;

using Player;


[RequireComponent(typeof(Animator))]
public class HatAnimation : MonoBehaviour {

    // Component interface

    public GameObject TargetArmature;
    public string TargetBone = "b_head";
    public Player.PlayerController m_PlayerController;

    private Animator m_Animator;    
    private Player_Movement m_PlayerMovement;
    private Transform m_HeadBone;

	// Use this for initialization
	void Start () {
        m_Animator = GetComponent<Animator>();

        m_PlayerMovement = m_PlayerController.GetComponent<Player_Movement>();
        
        m_PlayerController.OnDeath.AddListener( HandleDeath );
        
        //// Find target bone
        //m_HeadBone = TargetArmature.transform;
        //Transform tr = m_HeadBone.FindChild(TargetBone); ;
        //if (tr != null)
        //    m_HeadBone = tr;



        SetColor(m_PlayerController.PlayerColor);
    }
	
	// Update is called once per frame
	void Update () {

        // get player's speed, and input it into the animator
        float animSpeed = m_PlayerMovement.PreviousMovement.magnitude;
        m_Animator.SetFloat("Speed", animSpeed);

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    m_Animator.SetTrigger("PlayerDeath");
        //}
	}

    public void SetColor(Color color)
    {
        SkinnedMeshRenderer meshRenderer = GetComponentInChildren <SkinnedMeshRenderer>() as SkinnedMeshRenderer;

        if (meshRenderer != null)
            meshRenderer.material.SetColor("_Color", color);// = color;
    }

    private void HandleDeath()
    {
        m_PlayerController.OnDeath.RemoveListener(HandleDeath);
        //Debug.Log("Theoden King: DEEEEEEEEEEEAAAAAAAAATH!");
        m_Animator.SetTrigger("PlayerDeath");
        transform.SetParent(null, true);
    }
}
