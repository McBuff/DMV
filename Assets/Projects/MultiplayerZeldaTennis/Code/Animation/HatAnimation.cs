using UnityEngine;
using System.Collections;

using Player;


[RequireComponent(typeof(Animator))]
public class HatAnimation : MonoBehaviour {

    private Animator m_Animator;
    public Player.PlayerController m_PlayerController;
    private Player_Movement m_PlayerMovement;

	// Use this for initialization
	void Start () {
        m_Animator = GetComponent<Animator>();

        m_PlayerMovement = m_PlayerController.GetComponent<Player_Movement>();
    }
	
	// Update is called once per frame
	void Update () {

        // get player's speed, and input it into the animator
        float animSpeed = m_PlayerMovement.PreviousMovement.magnitude;
        m_Animator.SetFloat("Speed", animSpeed);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Animator.SetTrigger("PlayerDeath");
        }

        
	
	}
}
