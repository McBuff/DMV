using UnityEngine;
using System.Collections;

/// <summary>
/// This script reacts on trigger hits between weapons and players/deathballs, it sends a signal back to the owner
/// </summary>
public class WeaponCollisionManager : MonoBehaviour {

    private Player_Weapon m_Owner;

    // Use this for initialization
    void Start () {
        // set owned
        m_Owner = transform.parent.GetComponent<Player_Weapon>();
        if (m_Owner == null)
            Debug.LogError("This ");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider collider)
    {
        // let's see if a deathball has been hit
        DeathBall deathBall = collider.transform.GetComponent<DeathBall>();
        if (deathBall != null) {

            // some debug info
            Debug.Log("WeaponCollisionManager has hit a ball. Telling abll to be Hit!");

            // a deathball was hit, change its direction and increase its speed
            Vector3 newDirection = (deathBall.transform.position - m_Owner.transform.position);
            newDirection.y = 0f;// clamp y
            deathBall.DoBallHit(newDirection.normalized, collider.transform.position);

        }

        // let's see if a deathball has been hit
        BouncingProjectile dBall = collider.transform.GetComponent<BouncingProjectile>();
        if (dBall != null)
        {

            // some debug info
            //Debug.Log("WeaponCollisionManager has hit a ball. Telling ball to be Hit!");

            // a deathball was hit, change its direction and increase its speed
            Vector3 newDirection = (dBall.transform.position - m_Owner.transform.position);
            newDirection.y = 0f;// clamp y
            //dBall.DoBallHit(newDirection.normalized, collider.transform.position);
            dBall.photonView.RPC("BallHit", PhotonTargets.Others, new object[] { dBall.transform.position, newDirection , PhotonNetwork.time });
            dBall.BallHit(dBall.transform.position, newDirection, PhotonNetwork.time);

        }

    }
}
