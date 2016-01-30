using UnityEngine;
using System.Collections;

namespace Player
{
    public class FX_PlayerWalk : MonoBehaviour
    {
        private Player_Movement m_playerMovement;
        private ParticleSystem m_particleSystem;
        // Use this for initialization
        void Start()
        {
            m_playerMovement = GetComponentInParent<Player_Movement>();
            m_particleSystem = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            m_particleSystem.enableEmission = m_playerMovement.isMoving;
        }
    }
}